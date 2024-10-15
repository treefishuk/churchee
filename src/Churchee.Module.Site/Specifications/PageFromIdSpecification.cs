using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PageFromIdSpecification : Specification<Page>
    {

        public PageFromIdSpecification(Guid pageId)
        {
            Query.Where(x => x.Id == pageId);
        }

    }
}
