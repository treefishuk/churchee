using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Churchee.Module.x.Helpers;
using Churchee.Module.X.Exceptions;
using Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Churchee.Module.X.Jobs
{
    public class SyncTweets : IJob
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDataStore _dataStore;
        private readonly ISettingStore _settingStore;

        public SyncTweets(IHttpClientFactory httpClientFactory, IDataStore dataStore, ISettingStore settingStore)
        {
            _httpClientFactory = httpClientFactory;
            _dataStore = dataStore;
            _settingStore = settingStore;
        }

        public async Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            var tokenRepo = _dataStore.GetRepository<Token>();

            string bearerToken = await tokenRepo.FirstOrDefaultAsync(new GetTokenByKeySpecification(SettingKeys.XBearerToken, applicationTenantId), s => s.Value, cancellationToken);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            string userId = await _settingStore.GetSettingValue(Guid.Parse(SettingKeys.XUserId), applicationTenantId);

            string getTweetsUrl = await GetTweetsUrl(userId, applicationTenantId, cancellationToken);

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
                StoreTweet(applicationTenantId, mediaItemsRepo, tweetsMediaFolder, tweet);
            }

            await _dataStore.SaveChangesAsync(cancellationToken);
        }

        private async Task<string> GetTweetsUrl(string userId, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var mediaItemsRepo = _dataStore.GetRepository<MediaItem>();

            string lastTitle = await mediaItemsRepo.FirstOrDefaultAsync(new LatestTweetMediaItemSpecification(applicationTenantId), m => m.Title, cancellationToken);

            string sinceId = string.Empty;

            if (!string.IsNullOrEmpty(lastTitle))
            {
                // Title is in format "Tweet: {id}" - extract id
                string[] parts = lastTitle.Split([':'], 2);
                if (parts.Length == 2)
                {
                    sinceId = parts[1].Trim();
                }
            }

            return $"https://api.twitter.com/2/users/{userId}/tweets?tweet.fields=created_at&max_results=10" + (string.IsNullOrEmpty(sinceId) ? string.Empty : $"&since_id={sinceId}");
        }

        private static void StoreTweet(Guid applicationTenantId, IRepository<MediaItem> mediaItemsRepo, MediaFolder tweetsMediaFolder, Tweet tweet)
        {
            if (mediaItemsRepo.AnyWithFiltersDisabled(x => x.Title == $"Tweet: {tweet.Id}"))
            {
                return;
            }

            string tweetContent = tweet.Text;

            tweetContent = ReplaceLinkTextWithLinks(tweetContent);

            tweetContent = ReplaceUserHandlesWithLinks(tweetContent);

            var newTweet = new MediaItem(applicationTenantId, $"Tweet: {tweet.Id}", "/_content/Churchee.Module.X/img/tweet.png", "", tweetContent, tweetsMediaFolder.Id)
            {
                CreatedDate = tweet.CreatedAt
            };

            mediaItemsRepo.Create(newTweet);
        }

        private static string ReplaceUserHandlesWithLinks(string tweetContent)
        {
            string handlePattern = @"(?<!\S)@(\w+)"; // Ensure @username is not part of an email address

            string handleReplacement = "<a href=\"https://x.com/$1\">@$1</a>";

            tweetContent = Regex.Replace(tweetContent, handlePattern, handleReplacement, RegexOptions.None, TimeSpan.FromMilliseconds(500));

            return tweetContent;
        }

        private static string ReplaceLinkTextWithLinks(string tweetContent)
        {
            string linkPattern = @"(https?://[^\s]+)"; // Match HTTP/HTTPS URLs
            string linkReplacement = "<a href=\"$1\">$1</a>";

            tweetContent = Regex.Replace(tweetContent, linkPattern, linkReplacement, RegexOptions.None, TimeSpan.FromMilliseconds(500));

            return tweetContent;
        }
    }
}
