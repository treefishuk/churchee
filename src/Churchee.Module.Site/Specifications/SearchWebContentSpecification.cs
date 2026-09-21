using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class SearchWebContentSpecification : Specification<WebContent>
    {
        public SearchWebContentSpecification(string searchFilter)
        {
            if (!string.IsNullOrEmpty(searchFilter))
            {
                Query.Where(x => x.Title.Contains(searchFilter) || x.Url.Contains(searchFilter));
            }
        }
    }
}
