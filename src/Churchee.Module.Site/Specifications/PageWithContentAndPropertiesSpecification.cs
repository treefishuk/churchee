using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PageWithContentAndPropertiesSpecification : Specification<Page>
    {
        public PageWithContentAndPropertiesSpecification(Guid pageId)
        {
            Query.Include(i => i.PageContent)
                 .Include("PageContent.PageTypeContent")
                 .Where(x => x.Id == pageId);
        }

    }
}
