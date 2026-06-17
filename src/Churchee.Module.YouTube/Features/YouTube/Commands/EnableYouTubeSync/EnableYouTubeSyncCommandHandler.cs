using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.YouTube.Helpers;
using Churchee.Module.YouTube.Jobs;
using Churchee.Module.YouTube.Spotify.Features.YouTube.Commands;
using Hangfire;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync
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

            await _settingStore.AddOrUpdateSetting(SettingKeys.Handle, applicationTenantId, $"YouTubeHandle", request.ChannelIdentifier);

            var response = await StoreChannel(request, applicationTenantId, cancellationToken);

            if (!response.IsSuccess)
            {
                return response;
            }

            response = await StorePlaylist(request, applicationTenantId, cancellationToken);

            if (!response.IsSuccess)
            {
                return response;
            }

            var tokenRepo = _dataStore.GetRepository<Token>();

            tokenRepo.Create(new Token(applicationTenantId, SettingKeys.ApiKeyToken, request.ApiKey));

            await _dataStore.SaveChangesAsync(cancellationToken);

            try
            {
                _jobService.ScheduleJob<SyncYouTubeVideosJob>($"{applicationTenantId}_YouTubeVideos", a => a.ExecuteAsync(applicationTenantId, CancellationToken.None), Cron.Hourly);

                _jobService.QueueJob<FullSyncYouTubeVideosJob>(a => a.ExecuteAsync(applicationTenantId, CancellationToken.None));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing YouTube");

                response.AddError("Failed To Sync", "");
            }

            return response;
        }

        private async Task<CommandResponse> StoreChannel(EnableYouTubeSyncCommand request, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            if (!request.ChannelIdentifier.StartsWith('@'))
            {
                await _settingStore.AddOrUpdateSetting(SettingKeys.ChannelId, applicationTenantId, "YouTube Channel Id", request.ChannelIdentifier);

                return new CommandResponse();
            }

            var response = new CommandResponse();

            string getIdUrl = $"https://www.googleapis.com/youtube/v3/channels?part=id&forHandle={request.ChannelIdentifier}&key={request.ApiKey}";

            var httpClient = _httpClientFactory.CreateClient();

            var getChannelId = await httpClient.GetAsync(getIdUrl, cancellationToken);

            if (!getChannelId.IsSuccessStatusCode)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Failed to get channel ID from YouTube API: {StatusCode}", getChannelId.StatusCode);
                }

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

        private async Task<CommandResponse> StorePlaylist(EnableYouTubeSyncCommand request, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.PlaylistId))
            {
                await _settingStore.AddOrUpdateSetting(SettingKeys.Playlist, applicationTenantId, "YouTube Playlist Id", request.PlaylistId);

                return new CommandResponse();
            }

            var response = new CommandResponse();

            string channelId = await _settingStore.GetSettingValue(SettingKeys.ChannelId, applicationTenantId);

            string getPlaylistUrl = $"https://www.googleapis.com/youtube/v3/channels?part=contentDetails&id={channelId}&key={request.ApiKey}";

            var httpClient = _httpClientFactory.CreateClient();

            var getplaylistResponse = await httpClient.GetAsync(getPlaylistUrl, cancellationToken);

            if (!getplaylistResponse.IsSuccessStatusCode)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Failed to get Playlst ID from YouTube API: {StatusCode}", getplaylistResponse.StatusCode);
                }

                response.AddError("Failed to get channel ID from YouTube API", "");

                return response;
            }

            string json = await getplaylistResponse.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(json))
            {
                _logger.LogError("Failed to get playlist ID from YouTube API. Response was empty");

                response.AddError("Failed to get playlist ID from YouTube API", "");

                return response;
            }

            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            var playlist = root
                .GetProperty("items")[0]
                .GetProperty("contentDetails")
                .GetProperty("relatedPlaylists")
                .GetProperty("uploads")
                .GetString() ?? "unknown";

            if (playlist == "unknown")
            {
                _logger.LogError("Failed to get playlist ID from YouTube API. Response was empty");

                response.AddError("Failed to get playlist ID from YouTube API", "");

                return response;
            }

            await _settingStore.AddOrUpdateSetting(SettingKeys.Playlist, applicationTenantId, "YouTube Playlist Id", playlist);

            return response;
        }
    }
}
