using Churchee.Common.Storage;
using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;

namespace Churchee.Module.Dashboard.Jobs
{
    public class ClearOldPageViews
    {

        private readonly IDataStore _dataStore;

        public ClearOldPageViews(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task ClearPageViewsAsync(CancellationToken cancellationToken)
        {
            await _dataStore.GetRepository<PageView>().PermanentDelete(new OldPageViewSpecification(), cancellationToken);
        }
    }
}
