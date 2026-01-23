using Ardalis.Specification;
using Churchee.Module.Tenancy.Entities;

namespace Churchee.Module.Tenancy.Specifications
{
    public class ApplicationHostsByApplicationIdSpecification : Specification<ApplicationHost>
    {
        public ApplicationHostsByApplicationIdSpecification(Guid id)
        {
            Query.IgnoreQueryFilters().Where(w => w.Id == id);
        }
    }
}
