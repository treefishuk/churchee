using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageListingSpecificationTests
    {
        [Fact]
        public void Filters_By_Parent_And_Search()
        {
            var parentId = Guid.NewGuid();
            var p1 = new Page(Guid.NewGuid(), "Apple", "/a", "d", Guid.NewGuid(), parentId, false);
            var p2 = new Page(Guid.NewGuid(), "Banana", "/b", "d", Guid.NewGuid(), parentId, false);
            var p3 = new Page(Guid.NewGuid(), "Cherry", "/c", "d", Guid.NewGuid(), null, false);

            var list = new[] { p1, p2, p3 };

            var spec = new PageListingSpecification("App", parentId);

            var results = spec.Evaluate(list).ToList();

            _ = Assert.Single(results);
            Assert.Equal("Apple", results[0].Title);
        }
    }
}
