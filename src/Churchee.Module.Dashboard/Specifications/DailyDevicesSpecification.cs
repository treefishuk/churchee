using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class DailyDevicesSpecification : Specification<AnalyticsDailyDevices>
    {
        public DailyDevicesSpecification(DateTime startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => !string.IsNullOrEmpty(x.Device)
                   && x.Date > startDate);
        }

    }
}
