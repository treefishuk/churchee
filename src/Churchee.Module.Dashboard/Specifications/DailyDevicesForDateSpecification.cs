using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class DailyDevicesForDateSpecification : Specification<AnalyticsDailyDevices>
    {
        public DailyDevicesForDateSpecification(Guid applicationTenantId, DateOnly date, string device)
        {
            Query
                .IgnoreQueryFilters()
                .Where(x => x.ApplicationTenantId == applicationTenantId
                   && x.Date == date
                   && x.Device == device);
        }

    }
}
