using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PagesPageOfResultSpecificationTests
    {
        [Fact]
        public void Applies_Search_Order_And_Paging_With_Hierarchy()
        {
            var parentId = Guid.NewGuid();
            var p1 = new Page(Guid.NewGuid(), "Apple", "/a", "d", Guid.NewGuid(), parentId, false);
            var child = new Page(Guid.NewGuid(), "ChildApple", "/a/c", "d", Guid.NewGuid(), p1.Id, false);
            p1.Children = [child];

            var p2 = new Page(Guid.NewGuid(), "Banana", "/b", "d", Guid.NewGuid(), parentId, false);

            var list = new[] { p1, p2 };

            var spec = new PagesPageOfResultSpecification("ChildApple", 10, 0, "TITLE", "ASC", parentId);

            var results = spec.Evaluate(list).ToList();

            _ = Assert.Single(results);
            Assert.Equal(p1.Id, results[0].Id);
        }
    }
}
