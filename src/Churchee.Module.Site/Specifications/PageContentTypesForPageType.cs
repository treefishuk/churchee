using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PageContentTypesForPageType : Specification<WebContentTypeContent>
    {
        public PageContentTypesForPageType(Guid pageTypeId)
        {
            Query.Where(x => x.WebContentType.Id == pageTypeId)
                .OrderBy(o => o.Order);
        }

    }
}
