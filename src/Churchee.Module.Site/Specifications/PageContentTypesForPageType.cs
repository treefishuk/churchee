using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PageContentTypesForPageType : Specification<PageTypeContent>
    {
        public PageContentTypesForPageType(Guid pageTypeId)
        {
            Query.Where(x => x.PageType.Id == pageTypeId)
                .OrderBy(o => o.Order);
        }

    }
}
