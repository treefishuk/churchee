using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PageContentForPageSpecification : Specification<PageContent>
    {
        public PageContentForPageSpecification(Guid pageId)
        {
            Query.Where(x => x.Id == pageId).OrderBy(o => o.PageTypeContent.Name);
        }

    }
}
