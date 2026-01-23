using Ardalis.Specification;
using Churchee.Module.Tenancy.Entities;

namespace Churchee.Module.Tenancy.Specifications
{
    public class ApplicationTenantByIdSpecification : Specification<ApplicationTenant>
    {
        public ApplicationTenantByIdSpecification(Guid id)
        {
            Query.IgnoreQueryFilters().Where(w => w.Id == id);
        }
    }
}
