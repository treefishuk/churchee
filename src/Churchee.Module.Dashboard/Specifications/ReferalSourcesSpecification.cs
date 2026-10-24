using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class ReferralSourcesSpecification : Specification<AnalyticsDailyReferrers>
    {

        public ReferralSourcesSpecification(DateOnly startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => !string.IsNullOrEmpty(x.Referrer)
                    && !x.Referrer.StartsWith("/.")
                    && x.Date >= startDate);
        }

    }
}
