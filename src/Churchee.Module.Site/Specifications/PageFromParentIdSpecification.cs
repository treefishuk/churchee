using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PageFromParentIdSpecification : Specification<Page>
    {

        public PageFromParentIdSpecification(Guid parentPageId)
        {
            Query.Where(x => x.ParentId == parentPageId);
        }

    }
}
