using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    internal class PageTypeWithPageTypeContentSpecification : Specification<PageType>
    {
        public PageTypeWithPageTypeContentSpecification(Guid pageTypeId)
        {
            Query.Where(x => x.Id == pageTypeId).Include(i => i.PageTypeContent);

        }
    }
}
