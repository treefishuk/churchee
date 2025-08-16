using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class UniqueVisitorsSpecification : Specification<PageView>
    {

        public UniqueVisitorsSpecification(DateTime startDate, IQueryable<PageView> notInQuery)
        {
            Query
                .AsNoTracking()
                .Where(x => x.ViewedAt > startDate && !notInQuery.Contains(x));
        }

    }
}
