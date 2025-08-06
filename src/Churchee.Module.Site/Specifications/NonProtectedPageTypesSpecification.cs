using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class NonProtectedPageTypesSpecification : Specification<PageType>
    {
        public NonProtectedPageTypesSpecification()
        {
            Query.Where(w => !Helpers.PageTypes.AllButBlogListingTypes.Contains(w.SystemKey.Value));
        }
    }
}
