using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    internal class PageTypeContentPagingSpecification : Specification<PageTypeContent>
    {
        public PageTypeContentPagingSpecification(Guid pageTypeId, string searchText, int take, int skip, string orderByField, string orderDir)
        {
            Query.Where(w => w.PageType.Id == pageTypeId);
            ApplySearchFilter(searchText);
            ApplyOrdering(orderByField, orderDir);
            ApplyPaging(take, skip);
        }

        private void ApplySearchFilter(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                Query.Where(x => x.Name.Contains(search));
            }
        }

        private void ApplyOrdering(string orderByField, string orderDir)
        {
            if (string.IsNullOrEmpty(orderByField) || string.IsNullOrEmpty(orderDir))
            {
                return;
            }
            if (orderByField.ToUpperInvariant() == "NAME" && orderDir.ToUpperInvariant().StartsWith("ASC"))
            {
                Query.OrderBy(x => x.Name);
                return;
            }
            if (orderByField.ToUpperInvariant() == "NAME")
            {
                Query.OrderByDescending(x => x.Name);
                return;
            }

        }

        private void ApplyPaging(int take, int skip)
        {
            if (take == 0)
            {
                return;
            }

            Query.Skip(skip).Take(take);
        }

    }
}
