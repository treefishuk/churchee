using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class TotalViewsSpecification : Specification<PageView>
    {

        public TotalViewsSpecification(DateTime startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => x.ViewedAt > startDate
                    && !string.IsNullOrEmpty(x.Device)
                    && !string.IsNullOrEmpty(x.UserAgent)
                    && !x.UserAgent.Contains("Uptime-Kuma")
                    && !x.UserAgent.Contains("uptimerobot"));
        }

    }
}
