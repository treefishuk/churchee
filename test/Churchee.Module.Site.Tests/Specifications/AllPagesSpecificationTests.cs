using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class AllPagesSpecificationTests
    {
        [Fact]
        public void Specification_Returns_All()
        {
            var p1 = new Page(Guid.NewGuid(), "A", "/a", "d", Guid.NewGuid(), null, false);
            var p2 = new Page(Guid.NewGuid(), "B", "/b", "d", Guid.NewGuid(), null, false);

            var list = new[] { p1, p2 };

            var spec = new AllPagesSpecification();

            var results = spec.Evaluate(list).ToList();

            Assert.Equal(2, results.Count);
        }
    }
}
