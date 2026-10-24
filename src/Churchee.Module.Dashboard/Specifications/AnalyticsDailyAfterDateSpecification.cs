using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class AnalyticsDailyAfterDateSpecification : Specification<AnalyticsDailyPages>
    {

        public AnalyticsDailyAfterDateSpecification(DateOnly startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => x.Date >= startDate);
        }
    }
}
