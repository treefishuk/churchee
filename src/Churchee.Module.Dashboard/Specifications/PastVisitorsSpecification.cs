using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class PastVisitorsSpecification : Specification<PageView>
    {

        public PastVisitorsSpecification(DateTime startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => x.ViewedAt < startDate);
        }

    }
}
