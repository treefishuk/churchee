using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PagesAndArticlesSpecification : Specification<WebContent>
    {
        public PagesAndArticlesSpecification()
        {
            Query.Where(w => w is Page || w is Article);
        }
    }
}
