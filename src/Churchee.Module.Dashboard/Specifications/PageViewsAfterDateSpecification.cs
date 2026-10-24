using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class PageViewsAfterDateSpecification : Specification<PageView>
    {

        public PageViewsAfterDateSpecification(DateTime startDate)
        {
            Query
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => x.ViewedAt >= startDate);
        }
    }
}
