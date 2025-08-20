using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    internal class RedirectsSpecification : Specification<RedirectUrl>
    {
        public RedirectsSpecification(string searchText, Guid applicationTenantId)
        {
            Query.Where(w => w.WebContent.ApplicationTenantId == applicationTenantId);

            if (!string.IsNullOrEmpty(searchText))
            {
                Query.Where(w => w.Path.Contains(searchText));
            }
        }
    }
}
