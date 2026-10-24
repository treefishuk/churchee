using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class PageViewsAfterDateSpecification : Specification<AnalyticsDailyPages>
    {

        public PageViewsAfterDateSpecification(DateTime startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => x.Date > startDate);
        }
    }
}
