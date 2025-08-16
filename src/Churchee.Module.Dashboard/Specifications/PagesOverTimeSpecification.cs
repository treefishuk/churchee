using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class PagesOverTimeSpecification : Specification<PageView>
    {

        public PagesOverTimeSpecification(DateTime startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => x.ViewedAt > startDate);
        }

    }
}
