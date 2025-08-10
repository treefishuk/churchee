using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PagesWithNonProtectedPageTypesSpecification : Specification<Page>
    {
        public PagesWithNonProtectedPageTypesSpecification(Guid applicationTenantId, Guid? currentPageId)
        {
            Query.Include(i => i.PageType)
                 .IgnoreQueryFilters()
                 .Where(w => !Helpers.PageTypes.AllReservedType.Contains(w.PageType.SystemKey.Value)
                     && w.ApplicationTenantId == applicationTenantId
                     && w.Url != "/");

            if (currentPageId != null)
            {
                Query.Where(w => w.Id != currentPageId);
            }
        }
    }
}
