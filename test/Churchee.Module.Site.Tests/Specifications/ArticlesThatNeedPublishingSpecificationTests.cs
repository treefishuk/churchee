using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class ArticlesThatNeedPublishingSpecificationTests
    {
        [Fact]
        public void Filters_Unpublished_With_LastPublishedDate_LessOrEqual_Now()
        {
            var t = Guid.NewGuid();
            var a1 = new Article(t, Guid.NewGuid(), Guid.NewGuid(), "T", "/t", "d");
            a1.SetPublishDate(DateTime.UtcNow.AddDays(-1));
            // unpublished but LastPublishedDate <= now - should be included
            a1.UnPublish();

            var a2 = new Article(t, Guid.NewGuid(), Guid.NewGuid(), "T2", "/t2", "d");
            a2.SetPublishDate(null);
            a2.UnPublish();

            var list = new[] { a1, a2 };

            var spec = new ArticlesThatNeedPublishingSpecification();

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
            Assert.Contains(results, r => r.Title == "T");
        }
    }
}
