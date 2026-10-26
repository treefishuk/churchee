using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class PageViewsAfterDateBatchSpecification : Specification<PageView>
    {

        public PageViewsAfterDateBatchSpecification(DateTime startDate, int take, int skip)
        {
            Query
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => x.ViewedAt > startDate)
                .OrderBy(x => x.ViewedAt)
                .ThenBy(x => x.Id)
                .Skip(skip)
                .Take(take);
        }
    }
}
