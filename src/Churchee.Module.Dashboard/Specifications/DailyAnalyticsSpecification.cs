using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class DailyAnalyticsSpecification : Specification<AnalyticsDaily>
    {
        public DailyAnalyticsSpecification(DateTime startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => x.Date > startDate);
        }

    }
}
