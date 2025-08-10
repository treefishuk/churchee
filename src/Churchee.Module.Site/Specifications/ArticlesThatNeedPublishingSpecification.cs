using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class ArticlesThatNeedPublishingSpecification : Specification<Article>
    {

        public ArticlesThatNeedPublishingSpecification()
        {
            Query.Where(article => !article.Published && article.LastPublishedDate != null && article.LastPublishedDate <= DateTime.UtcNow);
        }

    }
}
