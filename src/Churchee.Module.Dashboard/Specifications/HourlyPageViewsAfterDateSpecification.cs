using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class HourlyPageViewsAfterDateSpecification : Specification<AnalyticsHourly>
    {
        public HourlyPageViewsAfterDateSpecification(DateOnly startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => x.Date >= startDate);
        }

    }
}
