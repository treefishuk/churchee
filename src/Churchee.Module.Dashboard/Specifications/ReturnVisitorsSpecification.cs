using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class ReturnVisitorsSpecification : Specification<PageView>
    {

        public ReturnVisitorsSpecification(DateTime startDate, IQueryable<string> inQuery)
        {
            Query
                .AsNoTracking()
                .Where(x => x.ViewedAt > startDate)
                .Where(x => !string.IsNullOrEmpty(x.Device)
                    && !string.IsNullOrEmpty(x.UserAgent)
                    && !x.UserAgent.Contains("Uptime-Kuma")
                    && !x.UserAgent.Contains("uptimerobot"))
                .Where(x => inQuery.Contains(x.IpAddress));
        }

    }
}
