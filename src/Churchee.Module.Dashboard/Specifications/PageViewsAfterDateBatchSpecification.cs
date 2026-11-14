using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class PageViewsAfterDateBatchSpecification : Specification<PageView>
    {

        public PageViewsAfterDateBatchSpecification(DateTime startDate, int take, DateTime? lastViewedAt = null, Guid? lastId = null)
        {
            Query
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => x.ViewedAt >= startDate);

            if (lastViewedAt.HasValue && lastId.HasValue)
            {
                Query.Where(x => x.ViewedAt > lastViewedAt.Value ||
                    x.ViewedAt == lastViewedAt.Value && x.Id.CompareTo(lastId.Value) > 0);
            }

            Query.OrderBy(x => x.ViewedAt)
                .ThenBy(x => x.Id)
                .Take(take);
        }
    }
}
