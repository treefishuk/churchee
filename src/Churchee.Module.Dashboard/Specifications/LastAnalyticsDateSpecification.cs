using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class LastAnalyticsDateSpecification : Specification<AnalyticsDaily>
    {

        public LastAnalyticsDateSpecification()
        {
            Query
                .IgnoreQueryFilters()
                .AsNoTracking()
                .OrderByDescending(o => o.Date);
        }
    }
}
