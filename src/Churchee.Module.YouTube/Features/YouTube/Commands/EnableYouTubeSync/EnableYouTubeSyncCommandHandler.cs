using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
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
using Churchee.Module.YouTube.Spotify.Features.YouTube.Commands;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Churchee.Module.YouTube.Features.YouTube.Commands
{
    public class EnableYouTubeSyncCommandHandler : IRequestHandler<EnableYouTubeSyncCommand, CommandResponse>
    {
        private readonly ISettingStore _settingStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IJobService _jobService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public EnableYouTubeSyncCommandHandler(ISettingStore settingStore, ICurrentUser currentUser, IDataStore dataStore, IJobService jobService, IHttpClientFactory httpClientFactory, ILogger<EnableYouTubeSyncCommandHandler> logger)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
            _dataStore = dataStore;
            _jobService = jobService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(EnableYouTubeSyncCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await _settingStore.AddOrUpdateSetting(SettingKeys.Handle, applicationTenantId, $"YouTubeHandle", request.Handle);

            string pageNameForVideos = await _settingStore.GetSettingValue(SettingKeys.VideosPageName, applicationTenantId);

            var tokenRepo = _dataStore.GetRepository<Token>();

            tokenRepo.Create(new Token(applicationTenantId, SettingKeys.ApiKeyToken, request.ApiKey));

            await _dataStore.SaveChangesAsync(cancellationToken);

            var response = await StoreChannelId(request, applicationTenantId, cancellationToken);

            if (!response.IsSuccess)
            {
                return response;
            }

            _jobService.ScheduleJob($"{applicationTenantId}_YouTubeVideos", () => SyncVideos(request, applicationTenantId, pageNameForVideos, CancellationToken.None), Cron.Hourly);

            _jobService.QueueJob(() => SyncVideos(request, applicationTenantId, pageNameForVideos, CancellationToken.None));

            return new CommandResponse();
        }

        private async Task<CommandResponse> StoreChannelId(EnableYouTubeSyncCommand request, Guid applicationTenantId, CancellationToken cancellationToken)
        {

            var response = new CommandResponse();

            string getIdUrl = $"https://www.googleapis.com/youtube/v3/channels?part=id&forHandle=@{request.Handle}&key={request.ApiKey}";

            var httpClient = _httpClientFactory.CreateClient();

            var getChannelId = await httpClient.GetAsync(getIdUrl, cancellationToken);

            if (!getChannelId.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get channel ID from YouTube API: {StatusCode}", getChannelId.StatusCode);

                response.AddError("Failed to get channel ID from YouTube API", "");

                return response;
            }

            string getChannelIdResponseString = await getChannelId.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(getChannelIdResponseString))
            {
                _logger.LogError("Failed to get channel ID from YouTube API. Response was empty");

                response.AddError("Failed to get channel ID from YouTube API", "");

                return response;
            }

            var getUserIdResponseClass = JsonSerializer.Deserialize<GetChannelIdApiResponse>(getChannelIdResponseString);

            if (getUserIdResponseClass == null)
            {
                _logger.LogError("Failed to deserialize Channel ID from YouTube API response");

                response.AddError("Failed to deserialize Channel ID from YouTube API response", "");

                return response;
            }

            string channelId = getUserIdResponseClass.ChannelId;

            await _settingStore.AddOrUpdateSetting(SettingKeys.ChannelId, applicationTenantId, "YouTube Channel Id", channelId);

            return response;
        }

        public async Task SyncVideos(EnableYouTubeSyncCommand request, Guid applicationTenantId, string videosUrl, CancellationToken cancellationToken)
        {
            string channelId = await _settingStore.GetSettingValue(SettingKeys.ChannelId, applicationTenantId);

            string videosPath = await _settingStore.GetSettingValue(SettingKeys.VideosPageName, applicationTenantId);

            var tokenRepo = _dataStore.GetRepository<Token>();

            string apiKey = await tokenRepo.FirstOrDefaultAsync(new GetTokenByKeySpecification(SettingKeys.ApiKeyToken, applicationTenantId), s => s.Value, cancellationToken);

            string getVideosUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&channelId={channelId}&order=date&type=video&maxResults=10&key={apiKey}";

            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(getVideosUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new YouTubeSyncException();
            }

            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            var deserializedResponse = JsonSerializer.Deserialize<GetYouTubeVideosApiResponse>(responseBody);

            if (deserializedResponse == null)
            {
                throw new YouTubeSyncException("Failed to deserialize YouTube API response");
            }

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
