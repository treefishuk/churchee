using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.API;
using Churchee.Module.Google.Reviews.Exceptions;
using Churchee.Module.Google.Reviews.Helpers;
using Churchee.Module.Reviews.Entities;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Churchee.Module.Google.Reviews.Jobs
{
    public class GoogleReviewsSyncJob : IJob
    {
        private readonly IDataStore _dataStore;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISettingStore _settingStore;
        private readonly IImageProcessor _imageProcessor;
        private readonly IBlobStore _blobStore;
        private readonly ILogger _logger;
        private readonly string _mybusinessaccountmanagementUri;
        private readonly string _mybusinessbusinessinformationUri;
        private readonly string _mybusinessUri;

        public GoogleReviewsSyncJob(IDataStore dataStore, IHttpClientFactory clientFactory, ISettingStore settingStore, IImageProcessor imageProcessor, IBlobStore blobStore, ILogger<GoogleReviewsSyncJob> logger, IConfiguration configuration)
        {
            _dataStore = dataStore;
            _clientFactory = clientFactory;
            _settingStore = settingStore;
            _imageProcessor = imageProcessor;
            _blobStore = blobStore;
            _logger = logger;

            var googleSection = configuration.GetSection("Google");

            _mybusinessaccountmanagementUri = googleSection.GetValue<string>("mybusinessaccountmanagementUri");
            _mybusinessbusinessinformationUri = googleSection.GetValue<string>("mybusinessbusinessinformationUri");
            _mybusinessUri = googleSection.GetValue<string>("mybusinessUri");

        }

        public async Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken)
        {

            var tokenRepo = _dataStore.GetRepository<Token>();

            var accessToken = await tokenRepo.FirstOrDefaultAsync(new GetTokenByKeySpecification(SettingKeys.GoogleReviewsAccessTokenKey.ToString(), applicationTenantId), cancellationToken);

            if (accessToken.CreatedDate < DateTime.Now.AddHours(-1))
            {
                accessToken = await RefreshAccessToken(accessToken, applicationTenantId, cancellationToken);
            }

            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);

            string accountId = await GetAccountId(client, cancellationToken);

            string locationId = await GetLocationId(client, accountId, applicationTenantId, cancellationToken);

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
                throw new GoogleReviewSyncException("Failed to refresh Google access token.");
            }

            var updatedToken = new Token(applicationTenantId, SettingKeys.GoogleReviewsAccessTokenKey.ToString(), credential.Token.AccessToken);

            tokenRepo.Update(updatedToken);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return updatedToken;
        }

        private async Task ProcessReviews(Guid applicationTenantId, GoogleReviewsResponse result, CancellationToken cancellationToken)
        {
            var repo = _dataStore.GetRepository<Review>();

            foreach (var review in result.Reviews)
            {

                if (repo.AnyWithFiltersDisabled(f => f.SourceId == review.Name && f.ApplicationTenantId == applicationTenantId))
                {
                    continue;
                }

                var entity = new Review(applicationTenantId)
                {
                    Comment = review.Comment,
                    ReviewerName = review.Reviewer.DisplayName,
                    ReviewerImageUrl = review.Reviewer.ProfilePhotoUrl,
                    Rating = (int)review.StarRating,
                    SourceName = "Google",
                    SourceId = review.Name,
                    CreatedDate = review.CreateTime
                };

                await ConvertImageToLocalImage(entity, cancellationToken);

                repo.Create(entity);
            }

            await _dataStore.SaveChangesAsync(cancellationToken);
        }

        internal async Task<GoogleReviewsResponse> GetReviews(HttpClient client, string accountId, string locationId, CancellationToken cancellationToken)
        {
            string url = $"{_mybusinessUri}accounts/{accountId}/locations/{locationId}/reviews";

            var response = await client.GetAsync(url, cancellationToken);

            string json = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer.Deserialize<GoogleReviewsResponse>(json);

            return result;
        }

        internal async Task<string> GetAccountId(HttpClient httpClient, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync($"{_mybusinessaccountmanagementUri}accounts", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new GoogleReviewSyncException($"Failed to retrieve account information. Status code: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync(cancellationToken);

            var accounts = JsonSerializer.Deserialize<AccountsResponse>(json);

            return accounts.Accounts.FirstOrDefault()?.Name.Split('/').LastOrDefault() ?? string.Empty;
        }

        internal async Task<string> GetLocationId(HttpClient httpClient, string accountId, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            string url = $"{_mybusinessbusinessinformationUri}accounts/{accountId}/locations?readMask=name,title";

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new GoogleReviewSyncException($"Failed to retrieve location information. Status code: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync(cancellationToken);

            var locationsResponse = JsonSerializer.Deserialize<LocationsResponse>(json);

            string targetAccountId = await _settingStore.GetSettingValue(SettingKeys.BusinessProfileId, applicationTenantId);

            string match = $"locations/{targetAccountId}";

            return locationsResponse.Locations.FirstOrDefault(f => f.Name == match)?.Name.Split('/').LastOrDefault() ?? string.Empty;
        }

        internal async Task ConvertImageToLocalImage(Review review, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(review.ReviewerImageUrl))
            {
                return;
            }

            var response = await _clientFactory.CreateClient().GetAsync(review.ReviewerImageUrl, cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var tempFileStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            try
            {
                using var webPStream = await _imageProcessor.ConvertToWebP(tempFileStream, cancellationToken);

                string friendlyFileName = $"{review.Id}.webp";

                string finalImagePath = await _blobStore.SaveAsync(review.ApplicationTenantId, $"/img/reviews/{friendlyFileName}", webPStream, true, cancellationToken);

                review.ReviewerImageUrl = finalImagePath;
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Error processing Google User image for review {ReviewId}", review.Id);
                }
            }
        }
    }
}
