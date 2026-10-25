using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class DailyPagesForTenantSpecification : Specification<AnalyticsDailyPages>
    {
        public DailyPagesForTenantSpecification(Guid applicationTenantId, DateOnly date, string url)
        {
            Query
                .IgnoreQueryFilters()
                .Where(x => x.ApplicationTenantId == applicationTenantId
                   && x.Date == date
                   && x.Url == url);
        }
    }
}
