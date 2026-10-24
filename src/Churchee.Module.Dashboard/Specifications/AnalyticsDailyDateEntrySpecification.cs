using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class AnalyticsDailyDateEntrySpecification : Specification<AnalyticsDaily>
    {

        public AnalyticsDailyDateEntrySpecification(DateOnly startDate, Guid applicationTenantId)
        {
            Query
                .AsNoTracking()
                .Where(x => x.Date == startDate && x.ApplicationTenantId == applicationTenantId);
        }
    }
}
