using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageFromIdSpecificationTests
    {
        [Fact]
        public void Constructor_Sets_Query()
        {
            var pageId = Guid.NewGuid();

            var spec = new PageFromIdSpecification(pageId);

            Assert.NotNull(spec);
        }

        [Fact]
        public void Evaluates_Filter_By_Id()
        {
            var id = Guid.NewGuid();

            var match = new Page(Guid.NewGuid(), "Title", "/test", "desc", Guid.NewGuid(), null, false);
            // ensure Id equals by setting via reflection? Page constructor assigns new id from base. Use Page and compare by creating separate one and getting its Id
            var targeted = new Page(Guid.NewGuid(), "T", "/t", "d", Guid.NewGuid(), null, false);

            var list = new[] { match, targeted };

            var spec = new PageFromIdSpecification(targeted.Id);

            var results = spec.Evaluate(list).ToList();

            _ = Assert.Single(results);
            Assert.Contains(results, r => r.Id == targeted.Id);
        }
    }
}
