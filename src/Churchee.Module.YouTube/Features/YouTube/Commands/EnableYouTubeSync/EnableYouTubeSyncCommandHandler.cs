using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.YouTube.Helpers;
using Churchee.Module.YouTube.Jobs;
using Churchee.Module.YouTube.Spotify.Features.YouTube.Commands;
using Hangfire;
using MediatR;
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

            var tokenRepo = _dataStore.GetRepository<Token>();

            tokenRepo.Create(new Token(applicationTenantId, SettingKeys.ApiKeyToken, request.ApiKey));

            await _dataStore.SaveChangesAsync(cancellationToken);

            var response = await StoreChannelId(request, applicationTenantId, cancellationToken);

            if (!response.IsSuccess)
            {
                return response;
            }

            try
            {
                _jobService.ScheduleJob<SyncYouTubeVideosJob>($"{applicationTenantId}_YouTubeVideos", a => a.ExecuteAsync(applicationTenantId, CancellationToken.None), Cron.Hourly);

                _jobService.QueueJob<FullSyncYouTubeVideosJob>(a => a.ExecuteAsync(applicationTenantId, CancellationToken.None));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing Facebook events");

                response.AddError("Failed To Sync", "");
            }

            return response;
        }


        private async Task<CommandResponse> StoreChannelId(EnableYouTubeSyncCommand request, Guid applicationTenantId, CancellationToken cancellationToken)
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
    }
}
