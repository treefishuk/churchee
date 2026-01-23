using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.YouTube.Jobs
{
    public class SyncYouTubeVideosJob : SyncYouTubeVideosJobBase, IJob
    {
        public SyncYouTubeVideosJob(ISettingStore settingStore, IDataStore dataStore, ICurrentUser currentUser, IJobService jobService, IHttpClientFactory httpClientFactory, ILogger<SyncYouTubeVideosJob> logger)
            : base(settingStore, dataStore, currentUser, jobService, httpClientFactory, logger)
        {
        }

        public async Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            await SyncVideos(applicationTenantId, 10, cancellationToken);
        }
    }
}
