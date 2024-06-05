using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    internal class PagesWithContentThatImplementPageTypeSpecification : Specification<Page>
    {
        public PagesWithContentThatImplementPageTypeSpecification(Guid pageTypeId)
        {
            Query.Include(i => i.PageContent).Where(w => w.PageTypeId == pageTypeId);
        }
    }
}
