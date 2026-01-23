using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Churchee.Module.Videos.Entities;
using Churchee.Module.Videos.Helpers;
using Churchee.Module.YouTube.Exceptions;
using Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync;
using Churchee.Module.YouTube.Helpers;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Churchee.Module.YouTube.Jobs
{
    public abstract class SyncYouTubeVideosJobBase
    {
        private readonly ISettingStore _settingStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IJobService _jobService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public SyncYouTubeVideosJobBase(ISettingStore settingStore, IDataStore dataStore, ICurrentUser currentUser, IJobService jobService, IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _settingStore = settingStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _jobService = jobService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected async Task SyncVideos(Guid applicationTenantId, int videoCount, CancellationToken cancellationToken)
        {
            string channelId = await _settingStore.GetSettingValue(SettingKeys.ChannelId, applicationTenantId);

            string videosPath = await _settingStore.GetSettingValue(SettingKeys.VideosPageName, applicationTenantId);

            var tokenRepo = _dataStore.GetRepository<Token>();

            string apiKey = await tokenRepo.FirstOrDefaultAsync(new GetTokenByKeySpecification(SettingKeys.ApiKeyToken, applicationTenantId), s => s.Value, cancellationToken);

            string getVideosUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&channelId={channelId}&order=date&type=video&maxResults={videoCount}&key={apiKey}";

            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(getVideosUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new YouTubeSyncException();
            }

            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            var deserializedResponse = JsonSerializer.Deserialize<GetYouTubeVideosApiResponse>(responseBody) ?? throw new YouTubeSyncException("Failed to deserialize YouTube API response");

            var videoRepo = _dataStore.GetRepository<Video>();

            foreach (var item in deserializedResponse.Items.Where(w => w.Snippet.ChannelId == channelId))
            {
                string videoUri = $"https://youtu.be/{item.Id.VideoId}";

                bool alreadyExists = videoRepo.AnyWithFiltersDisabled(w => w.VideoUri == videoUri && w.ApplicationTenantId == applicationTenantId);

                if (!alreadyExists)
                {
                    await AddNewVideo(applicationTenantId, videosPath, item, cancellationToken);
                }

                await _dataStore.SaveChangesAsync(cancellationToken);

            }
        }

        private async Task AddNewVideo(Guid applicationTenantId, string videosPath, YouTubeVideo item, CancellationToken cancellationToken)
        {
            var videoDetailPageTypeId = await _dataStore.GetRepository<PageType>().FirstOrDefaultAsync(new PageTypeFromSystemKeySpecification(PageTypes.VideoDetailPageTypeId, applicationTenantId), s => s.Id, cancellationToken);

            if (videoDetailPageTypeId == Guid.Empty)
            {
                throw new YouTubeSyncException("videoDetailPageTypeId is Empty");
            }

            var entityToAdd = new Video(applicationTenantId: applicationTenantId,
                videoUri: $"https://youtu.be/{item.Id.VideoId}",
                publishedDate: item.Snippet.PublishTime,
                sourceName: "YouTube",
                sourceId: item.Id.VideoId,
                title: WebUtility.HtmlDecode(item.Snippet.Title),
                description: WebUtility.HtmlDecode(item.Snippet.Description),
                thumbnailUrl: item.Snippet.Thumbnails.High.Url,
                videosPath: videosPath,
                pageTypeId: videoDetailPageTypeId);

            var videoRepo = _dataStore.GetRepository<Video>();

            videoRepo.Create(entityToAdd);
        }
    }
}
