using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.YouTube.Jobs
{
    public class FullSyncYouTubeVideosJob : SyncYouTubeVideosJobBase, IJob
    {
        public FullSyncYouTubeVideosJob(ISettingStore settingStore, IDataStore dataStore, ICurrentUser currentUser, IJobService jobService, IHttpClientFactory httpClientFactory, ILogger<FullSyncYouTubeVideosJob> logger)
            : base(settingStore, dataStore, currentUser, jobService, httpClientFactory, logger)
        {
        }

        public async Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            await SyncVideos(applicationTenantId, 1000, cancellationToken);
        }
    }
}
