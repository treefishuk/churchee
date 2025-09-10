using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    internal class GetPageTypeByIdAndIncludePageTypeContentSpecification : Specification<PageType>
    {
        public GetPageTypeByIdAndIncludePageTypeContentSpecification(Guid id)
        {
            Query.Include(i => i.PageTypeContent).Where(w => w.Id == id);
        }

    }
}
