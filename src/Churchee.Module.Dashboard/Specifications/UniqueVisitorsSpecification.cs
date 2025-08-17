using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class UniqueVisitorsSpecification : Specification<PageView>
    {

        public UniqueVisitorsSpecification(DateTime startDate, IQueryable<string> notInQuery)
        {
            Query
                .AsNoTracking()
                .Where(x => x.ViewedAt > startDate)
                .Where(x => !string.IsNullOrEmpty(x.Device)
                    && !string.IsNullOrEmpty(x.UserAgent)
                    && !x.UserAgent.Contains("Uptime-Kuma")
                    && !x.UserAgent.Contains("uptimerobot"))
                .Where(x => !notInQuery.Contains(x.IpAddress));
        }

    }
}
