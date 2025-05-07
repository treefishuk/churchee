using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Videos.Entities;
using Churchee.Module.YouTube.Helpers;
using MediatR;

namespace Churchee.Module.YouTube.Features.YouTube.Commands
{
    public class DisableYouTubeSyncCommandHandler : IRequestHandler<DisableYouTubeSyncCommand, CommandResponse>
    {
        private readonly ISettingStore _settingStore;
        private readonly Guid _rssFeedSettingKey = Guid.Parse("a9cd25bb-23b4-45ba-9484-04fc458ad29a");
        private readonly IJobService _jobService;
        private readonly ICurrentUser _currentUser;
        private readonly IDataStore _dataStore;


        public DisableYouTubeSyncCommandHandler(ISettingStore settingStore, ICurrentUser currentUser, IJobService jobService, IDataStore dataStore)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
            _jobService = jobService;
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(DisableYouTubeSyncCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await _settingStore.ClearSetting(SettingKeys.Handle, applicationTenantId);
            await _settingStore.ClearSetting(SettingKeys.ChannelId, applicationTenantId);

            _jobService.RemoveScheduledJob($"{applicationTenantId}_YouTubeVideos");

            var spotifyPodcasts = _dataStore.GetRepository<Video>().GetQueryable().Where(w => w.SourceName == "YouTube").ToList();

            foreach (var item in spotifyPodcasts)
            {
                _dataStore.GetRepository<Video>().PermanentDelete(item);
            }

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
