using Ardalis.Specification;
using Churchee.Module.Tenancy.Entities;

namespace Churchee.Module.Tenancy.Specifications
{
    public class ApplicationTenantByIdIncludingHostsSpecification : Specification<ApplicationTenant>
    {
        public ApplicationTenantByIdIncludingHostsSpecification(Guid id)
        {
            Query.IgnoreQueryFilters().Include(i => i.Hosts).Where(w => w.Id == id);
        }
    }
}
