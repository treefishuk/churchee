using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class WebContentFromIdAndTenantSpecification : Specification<WebContent>
    {

        public WebContentFromIdAndTenantSpecification(Guid applicationTenantId, Guid webContentId)
        {
            Query.IgnoreQueryFilters().Where(x => x.ApplicationTenantId == applicationTenantId && x.Id == webContentId);
        }
    }
}
