
using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    internal class CssForSpecifiedTenantSpecification : Specification<Css>
    {
        public CssForSpecifiedTenantSpecification(Guid applicationTenantId)
        {
            Query.IgnoreQueryFilters().Where(css => css.ApplicationTenantId == applicationTenantId);
        }

    }
}
