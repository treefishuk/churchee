using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    internal class ArticlePageOfResultSpecification : Specification<Article>
    {
        public ArticlePageOfResultSpecification(string searchText, int take, int skip, string orderByField, string orderDir)
        {
            ApplySearchFilter(searchText);
            ApplyOrdering(orderByField, orderDir);
            ApplyPaging(take, skip);
        }

        private void ApplySearchFilter(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                Query.Where(x => x.Title.Contains(search));
            }
        }

        private void ApplyOrdering(string orderByField, string orderDir)
        {
            if (string.IsNullOrEmpty(orderByField) || string.IsNullOrEmpty(orderDir))
            {
                return;
            }
            if (orderByField.Equals("TITLE", StringComparison.InvariantCultureIgnoreCase) && orderDir.StartsWith("ASC", StringComparison.InvariantCultureIgnoreCase))
            {
                Query.OrderBy(x => x.Title);
                return;
            }
            if (orderByField.Equals("TITLE", StringComparison.InvariantCultureIgnoreCase))
            {
                Query.OrderByDescending(x => x.Title);
                return;
            }
            if (orderByField.Equals("URL", StringComparison.InvariantCultureIgnoreCase) && orderDir.StartsWith("ASC", StringComparison.InvariantCultureIgnoreCase))
            {
                Query.OrderBy(x => x.Url);
                return;
            }
            if (orderByField.Equals("URL", StringComparison.InvariantCultureIgnoreCase))
            {
                Query.OrderByDescending(x => x.Url);
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
