using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class DailyRererersForTenantSpecification : Specification<AnalyticsDailyReferrers>
    {

        public DailyRererersForTenantSpecification(Guid applicationTenantId, DateOnly date, string referer)
        {
            Query
                .IgnoreQueryFilters()
                .Where(x => x.Date == date && x.ApplicationTenantId == applicationTenantId && x.Referrer == referer && !string.IsNullOrEmpty(x.Referrer));
        }
    }
}
