using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageFromParentIdSpecificationTests
    {
        [Fact]
        public void Filters_By_ParentId()
        {
            var parentId = Guid.NewGuid();
            var p1 = new Page(Guid.NewGuid(), "A", "/a", "d", Guid.NewGuid(), parentId, false);
            var p2 = new Page(Guid.NewGuid(), "B", "/b", "d", Guid.NewGuid(), null, false);

            var list = new[] { p1, p2 };

            var spec = new PageFromParentIdSpecification(parentId);

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
            Assert.Equal(parentId, results[0].ParentId);
        }
    }
}