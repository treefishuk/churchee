using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class ArticleFromIdSpecificationTests
    {
        [Fact]
        public void Constructor_Sets_Query_And_Filters_By_Id()
        {
            var target = new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "T", "/t", "d");
            var other = new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "O", "/o", "d");

            var list = new[] { other, target };

            var spec = new ArticleFromIdSpecification(target.Id);

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
            Assert.Contains(results, r => r.Id == target.Id);
        }
    }
}