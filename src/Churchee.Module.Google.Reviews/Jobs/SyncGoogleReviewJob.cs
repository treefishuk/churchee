using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.API;
using Churchee.Module.Google.Reviews.Exceptions;
using Churchee.Module.Google.Reviews.Helpers;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Churchee.Module.Google.Reviews.Jobs
{
    public class SyncGoogleReviewJob : IJob
    {
        private readonly IDataStore _dataStore;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISettingStore _settingStore;

        public SyncGoogleReviewJob(IDataStore dataStore, IHttpClientFactory clientFactory, ISettingStore settingStore)
        {
            _dataStore = dataStore;
            _clientFactory = clientFactory;
            _settingStore = settingStore;
        }

        public async Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var tokenRepo = _dataStore.GetRepository<Token>();

            var accessToken = await tokenRepo.ApplySpecification(new GetTokenByKeySpecification(SettingKeys.GoogleReviewsAccessTokenKey.ToString(), applicationTenantId)).FirstOrDefaultAsync(cancellationToken);

            if (accessToken.CreatedDate < DateTime.Now.AddHours(-1))
            {
                accessToken = await RefreshAccessToken(accessToken, applicationTenantId, cancellationToken);
            }

            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);

            string accountId = await _settingStore.GetSettingValue(SettingKeys.BusinessProfileId, applicationTenantId);

            string locationId = await GetLocationId(client, accountId, cancellationToken);

            var result = await GetReviews(client, accountId, locationId, cancellationToken);

            if (result.Reviews.Count <= 0)
            {
                return;
            }

            await ProcessReviews(applicationTenantId, result, cancellationToken);
        }

        private async Task<Token> RefreshAccessToken(Token accessToken, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var tokenRepo = _dataStore.GetRepository<Token>();

            var refreshToken = await tokenRepo.ApplySpecification(new GetTokenByKeySpecification(SettingKeys.GoogleReviewsRefreshTokenKey.ToString(), applicationTenantId)).FirstOrDefaultAsync(cancellationToken);

            string clientId = await _settingStore.GetSettingValue(SettingKeys.ClientId, applicationTenantId);
            string clientSecret = await _settingStore.GetSettingValue(SettingKeys.ClientSecret, applicationTenantId);
            string userId = accessToken.CreatedById?.ToString() ?? string.Empty;

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                Scopes = ["https://www.googleapis.com/auth/business.manage"],
            });

            var token = new TokenResponse
            {
                RefreshToken = refreshToken.Value
            };

            var credential = new UserCredential(flow, userId, token);

            bool success = await credential.RefreshTokenAsync(CancellationToken.None);

            if (!success)
            {
                throw new Exception("Failed to refresh Google access token.");
            }

            var updatedToken = new Token(applicationTenantId, SettingKeys.GoogleReviewsAccessTokenKey.ToString(), credential.Token.AccessToken);

            // Optionally: store the updated token back in your DB

            tokenRepo.Update(updatedToken);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return updatedToken;
        }

        private async Task ProcessReviews(Guid applicationTenantId, GoogleReviewsResponse result, CancellationToken cancellationToken)
        {
            var mediaItemRepo = _dataStore.GetRepository<MediaItem>();

            var mediaFolderRepo = _dataStore.GetRepository<MediaFolder>();

            var mediaFolder = await mediaFolderRepo.FirstOrDefaultAsync(new MediaFolderByNameSpecification("GoogleReviews", applicationTenantId), cancellationToken);

            if (mediaFolder == null)
            {
                mediaFolder = new MediaFolder(applicationTenantId, "GoogleReviews", string.Empty);
                mediaFolderRepo.Create(mediaFolder);
                await _dataStore.SaveChangesAsync(cancellationToken);
            }

            foreach (var review in result.Reviews)
            {
                string html = $"<span class=\"starrating\">{GenerateStarRatingHtml(review.StarRating)}</span>";

                var newItem = new MediaItem(applicationTenantId, review.Reviewer.DisplayName, review.Reviewer.ProfilePhotoUrl, review.Comment, html, mediaFolder.Id);

                mediaItemRepo.Create(newItem);
            }

            await _dataStore.SaveChangesAsync(cancellationToken);
        }

        private string GenerateStarRatingHtml(int starRating)
        {
            var html = new StringBuilder();

            for (int i = 0; i < 5; i++)
            {
                if (i < starRating)
                {
                    html.Append('★'); // Filled star
                }
                else
                {
                    html.Append('☆'); // Empty star
                }
            }

            return html.ToString();
        }

        private async Task<GoogleReviewsResponse> GetReviews(HttpClient client, string accountId, string locationId, CancellationToken cancellationToken)
        {
            string url = $"https://mybusiness.googleapis.com/v4/accounts/{accountId}/locations/{locationId}/reviews";

            var response = await client.GetAsync(url, cancellationToken);

            string json = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer.Deserialize<GoogleReviewsResponse>(json);

            return result;
        }

        private async Task<string> GetAccountId(HttpClient httpClient, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync("https://mybusinessaccountmanagement.googleapis.com/v1/accounts", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new GoogleReviewSyncException($"Failed to retrieve account information. Status code: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync(cancellationToken);

            var accounts = JsonSerializer.Deserialize<AccountsResponse>(json);

            return accounts.Accounts.FirstOrDefault()?.Name.Split('/').LastOrDefault() ?? string.Empty;
        }

        private async Task<string> GetLocationId(HttpClient httpClient, string accountId, CancellationToken cancellationToken)
        {
            string url = $"https://mybusinessbusinessinformation.googleapis.com/v1/accounts/{accountId}/locations";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new GoogleReviewSyncException($"Failed to retrieve location information. Status code: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync(cancellationToken);

            var locationsResponse = JsonSerializer.Deserialize<LocationsResponse>(json);

            return locationsResponse.Locations.FirstOrDefault()?.Name.Split('/').LastOrDefault() ?? string.Empty;
        }
    }
}
