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
            const int batchSize = 500;

            var repository = _dataStore.GetRepository<PageView>();

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            timeoutCts.CancelAfter(TimeSpan.FromMinutes(10));

            var token = timeoutCts.Token;

            int count = await repository.CountAsync(new OldPageViewSpecification(), token);

            while (count > 0)
            {
                var oldPageViews = await repository.GetListAsync(new OldPageViewSpecification(), token);

                var batch = oldPageViews?.Take(batchSize).ToList() ?? [];

                foreach (var pageView in batch)
                {
                    repository.PermanentDelete(pageView);
                }

                if (batch.Count > 0)
                {
                    await _dataStore.SaveChangesAsync(token);
                }

                count -= batch.Count;
            }
        }
    }
}
