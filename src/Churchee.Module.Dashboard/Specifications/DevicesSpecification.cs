using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class DevicesSpecification : Specification<PageView>
    {

        public DevicesSpecification(DateTime startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => !string.IsNullOrEmpty(x.Device)
                    && !string.IsNullOrEmpty(x.UserAgent)
                    && !x.UserAgent.Contains("Uptime-Kuma")
                    && !x.UserAgent.Contains("uptimerobot")
                    && x.ViewedAt > startDate);
        }

    }
}
