using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;

namespace Churchee.Module.YouTube.Jobs
{
    public class SyncYouTubeVideosJob : SyncYouTubeVideosJobBase, IJob
    {
        public SyncYouTubeVideosJob(ISettingStore settingStore, IDataStore dataStore, IHttpClientFactory httpClientFactory)
            : base(settingStore, dataStore, httpClientFactory)
        {
        }

        public async Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            await SyncVideos(applicationTenantId, 10, cancellationToken);
        }
    }
}
