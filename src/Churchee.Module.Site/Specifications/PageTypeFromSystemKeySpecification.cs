using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PageTypeFromSystemKeySpecification : Specification<PageType>
    {
        public PageTypeFromSystemKeySpecification(Guid systemKey)
        {
            Query.Where(x => x.SystemKey == systemKey);

        }
    }
}
