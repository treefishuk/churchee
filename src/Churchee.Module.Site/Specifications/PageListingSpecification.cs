using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class PageListingSpecification : Specification<Page>
    {

        public PageListingSpecification(string searchText, Guid? parentId)
        {
            Query.Where(w => w.ParentId == parentId);

            if (!string.IsNullOrEmpty(searchText))
            {
                Query.Where(w => w.Title.Contains(searchText));
            }
        }

    }
}
