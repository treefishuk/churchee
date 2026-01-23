using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    public class OldPageViewSpecification : Specification<PageView>
    {
        public OldPageViewSpecification()
        {
            var cutOffDate = DateTime.UtcNow.AddMonths(-3);

            Query.IgnoreQueryFilters().Where(x => x.ViewedAt <= cutOffDate);
        }
    }
}
