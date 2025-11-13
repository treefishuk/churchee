using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Churchee.Module.x.Helpers;
using Churchee.Module.X.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync
{
    public class EnableTweetsSyncCommandHandler : IRequestHandler<EnableTweetsSyncCommand, CommandResponse>
    {

        private readonly IJobService _jobService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly ISettingStore _settingStore;
        private readonly ILogger _logger;
        public EnableTweetsSyncCommandHandler(IJobService jobService, IHttpClientFactory httpClientFactory, IDataStore dataStore, ICurrentUser currentUser, ISettingStore settingStore, ILogger<EnableTweetsSyncCommandHandler> logger)
        {
            _jobService = jobService;
            _httpClientFactory = httpClientFactory;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _settingStore = settingStore;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(EnableTweetsSyncCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var tokenRepo = _dataStore.GetRepository<Token>();

            tokenRepo.Create(new Token(applicationTenantId, SettingKeys.XBearerToken, request.BearerToken));

            _ = await _dataStore.SaveChangesAsync(cancellationToken);

            string accountName = request.AccountName;

            if (accountName.StartsWith('@'))
            {
                accountName = accountName[1..];
            }

            await _settingStore.AddOrUpdateSetting(Guid.Parse(SettingKeys.XUserAccount), applicationTenantId, "X/Twitter UserName", accountName);

            var response = await StoreUserId(accountName, request.BearerToken, applicationTenantId, cancellationToken);

            if (!response.IsSuccess)
            {
                return response;
            }

            string newTemplatePath = "/Views/Shared/Components/Carousel/Tweets.cshtml";

            if (!_dataStore.GetRepository<ViewTemplate>().AnyWithFiltersDisabled(a => a.Location == newTemplatePath && a.ApplicationTenantId == applicationTenantId))
            {
                _dataStore.GetRepository<ViewTemplate>().Create(new ViewTemplate(applicationTenantId, newTemplatePath, ViewTemplateData.TweetListing));

                _ = await _dataStore.SaveChangesAsync(cancellationToken);
            }

            try
            {
                _jobService.ScheduleJob($"{applicationTenantId}_SyncTweets", () => SyncTweets(applicationTenantId, CancellationToken.None), () => "*/20 * * * *");
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Failed to schedule job for syncing tweets. Exception: {Exception}", ex.Message);
                }

                response.AddError("Failed to schedule job for syncing tweets", "");

                return response;
            }

            try
            {
                _jobService.QueueJob(() => SyncTweets(applicationTenantId, CancellationToken.None));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to queue job for syncing tweets. Exception: {Exception}", ex.Message);

                response.AddError("Failed to queue job for syncing tweets", "");

                return response;
            }

            return response;
        }

        public async Task<CommandResponse> StoreUserId(string accountName, string bearerToken, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            using var httpClient = _httpClientFactory.CreateClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            string getUserIdUrl = $"https://api.twitter.com/2/users/by/username/{accountName}";

            var getUserIdResponse = await httpClient.GetAsync(getUserIdUrl, cancellationToken);

            if (!getUserIdResponse.IsSuccessStatusCode)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Failed to get user ID from Twitter API. Status code: {StatusCode}", getUserIdResponse.StatusCode);
                }

                response.AddError("Failed to get user ID from Twitter API", "");

                return response;
            }

            string getUserIdResponseString = await getUserIdResponse.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(getUserIdResponseString))
            {
                _logger.LogError("Failed to get user ID from Twitter API. Response was empty");

                response.AddError("Failed to get user ID from Twitter API", "");

                return response;
            }

            var getUserIdResponseClass = JsonSerializer.Deserialize<GetAccountIdApiResponse>(getUserIdResponseString);

            if (getUserIdResponseClass == null || getUserIdResponseClass.Data == null || string.IsNullOrEmpty(getUserIdResponseClass.Data.Id))
            {
                _logger.LogError("Failed to deserialize user ID from Twitter API response");

                response.AddError("Failed to deserialize user ID from Twitter API response", "");

                return response;
            }

            string userId = getUserIdResponseClass.GetId();

            await _settingStore.AddOrUpdateSetting(Guid.Parse(SettingKeys.XUserId), applicationTenantId, "X/Twitter Identifier", userId);

            var mediaFolderRepo = _dataStore.GetRepository<MediaFolder>();

            var tweetsFolder = new MediaFolder(applicationTenantId, "Tweets", "");

            mediaFolderRepo.Create(tweetsFolder);

            _ = await _dataStore.SaveChangesAsync(cancellationToken);

            return response;
        }


        public async Task SyncTweets(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            var tokenRepo = _dataStore.GetRepository<Token>();

            string bearerToken = await tokenRepo.FirstOrDefaultAsync(new GetTokenByKeySpecification(SettingKeys.XBearerToken, applicationTenantId), s => s.Value, cancellationToken);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            string userId = await _settingStore.GetSettingValue(Guid.Parse(SettingKeys.XUserId), applicationTenantId);

            string getTweetsUrl = $"https://api.twitter.com/2/users/{userId}/tweets?tweet.fields=created_at&max_results=10";

            var response = await httpClient.GetAsync(getTweetsUrl, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new XSyncException($"Failed To Get Tweets. Response code: {response.StatusCode}");
            }

            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            var deserializedResponse = JsonSerializer.Deserialize<GetTweetsApiResponse>(responseBody);

            if (deserializedResponse == null)
            {
                throw new XSyncException("Failed to deserialize tweets from Twitter API response");
            }

            if (deserializedResponse.Tweets.Count == 0)
            {
                return;
            }

            var mediaItemsRepo = _dataStore.GetRepository<MediaItem>();
            var mediaFolderRepo = _dataStore.GetRepository<MediaFolder>();

            var tweetsMediaFolder = await mediaFolderRepo.FirstOrDefaultAsync(new MediaFolderByNameSpecification("Tweets", applicationTenantId), cancellationToken);

            foreach (var tweet in deserializedResponse.Tweets)
            {
                if (mediaItemsRepo.AnyWithFiltersDisabled(x => x.Title == $"Tweet: {tweet.Id}"))
                {
                    continue;
                }

                string tweetContent = tweet.Text;

                string linkPattern = @"(https?://[^\s]+)"; // Match HTTP/HTTPS URLs
                string linkReplacement = "<a href=\"$1\">$1</a>";

                tweetContent = Regex.Replace(tweetContent, linkPattern, linkReplacement, RegexOptions.None, TimeSpan.FromMilliseconds(500));

                string handlePattern = @"(?<!\S)@(\w+)"; // Ensure @username is not part of an email address
                string handleReplacement = "<a href=\"https://x.com/$1\">@$1</a>";

                tweetContent = Regex.Replace(tweetContent, handlePattern, handleReplacement, RegexOptions.None, TimeSpan.FromMilliseconds(500));

                var newTweet = new MediaItem(applicationTenantId, $"Tweet: {tweet.Id}", "/_content/Churchee.Module.X/img/tweet.png", "", tweetContent, tweetsMediaFolder.Id)
                {
                    CreatedDate = tweet.CreatedAt
                };

                mediaItemsRepo.Create(newTweet);
            }

            _ = await _dataStore.SaveChangesAsync(cancellationToken);
        }

    }

}
