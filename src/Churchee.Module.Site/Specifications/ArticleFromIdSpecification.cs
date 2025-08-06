using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class ArticleFromIdSpecification : Specification<Article>
    {
        public ArticleFromIdSpecification(Guid articleId)
        {
            Query.Where(w => w.Id == articleId);
        }
    }
}
