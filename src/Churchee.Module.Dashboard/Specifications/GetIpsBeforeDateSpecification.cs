using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class GetIpsBeforeDateSpecification : Specification<PageView>
    {
        public GetIpsBeforeDateSpecification(DateTime start)
        {
            Query.AsNoTracking().Where(x => x.ViewedAt <= start);
        }
    }
}
