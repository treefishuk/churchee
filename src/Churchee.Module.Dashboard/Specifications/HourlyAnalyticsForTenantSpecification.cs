using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class HourlyAnalyticsForTenantSpecification : Specification<AnalyticsHourly>
    {

        public HourlyAnalyticsForTenantSpecification(Guid applicationTenantId, DateOnly date, int hourOfDay)
        {
            Query
                .IgnoreQueryFilters()
                .Where(x => x.ApplicationTenantId == applicationTenantId && x.Date == date && x.HourOfDay == hourOfDay);
        }
    }
}
