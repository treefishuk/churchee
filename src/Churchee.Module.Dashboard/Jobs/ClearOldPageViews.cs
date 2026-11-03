using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;

namespace Churchee.Module.Dashboard.Jobs
{
    public class ClearOldPageViews : ISystemJob
    {
        private readonly IDataStore _dataStore;

        public ClearOldPageViews(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var repository = _dataStore.GetRepository<PageView>();

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            timeoutCts.CancelAfter(TimeSpan.FromMinutes(10));

            var token = timeoutCts.Token;

            const int batchSize = 2000;

            while (!token.IsCancellationRequested)
            {
                var deleted = await repository.PermanentDeleteBatchAsync(new OldPageViewSpecification(), batchSize, token);

                if (deleted == 0)
                {
                    break;
                }
            }
        }
    }
}
