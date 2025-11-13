using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class ArticlePageOfResultSpecificationTests
    {
        [Fact]
        public void Applies_Search_Sort_And_Paging()
        {
            var a1 = new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Apple", "/a", "d");
            var a2 = new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Banana", "/b", "d");

            var list = new[] { a2, a1 };

            var spec = new ArticlePageOfResultSpecification("App", 10, 0, "TITLE", "ASC");

            var results = spec.Evaluate(list).ToList();

            _ = Assert.Single(results);
            Assert.Equal("Apple", results.First().Title);
        }
    }
}
