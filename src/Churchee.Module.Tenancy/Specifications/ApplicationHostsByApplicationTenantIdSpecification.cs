using Ardalis.Specification;
using Churchee.Module.Tenancy.Entities;

namespace Churchee.Module.Tenancy.Specifications
{
    public class ApplicationHostsByApplicationTenantIdSpecification : Specification<ApplicationHost>
    {
        public ApplicationHostsByApplicationTenantIdSpecification(Guid applicationTenantId)
        {
            Query.IgnoreQueryFilters().Where(w => w.ApplicationTenantId == applicationTenantId);
        }
    }
}
