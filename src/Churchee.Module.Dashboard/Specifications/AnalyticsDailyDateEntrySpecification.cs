using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class AnalyticsDailyDateEntrySpecification : Specification<AnalyticsDaily>
    {

        public AnalyticsDailyDateEntrySpecification(DateOnly date, Guid applicationTenantId)
        {
            Query
                .IgnoreQueryFilters()
                .Where(x => x.Date == date && x.ApplicationTenantId == applicationTenantId);
        }
    }
}
