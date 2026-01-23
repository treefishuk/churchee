using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    internal class IncludePageTypeContentSpecification : Specification<PageType>
    {
        public IncludePageTypeContentSpecification()
        {
            Query.Include(i => i.PageTypeContent);
        }

    }
}
