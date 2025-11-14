using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PagesWithContentThatImplementPageTypeSpecificationTests
    {
        [Fact]
        public void Includes_PageContent_And_Filters_By_PageTypeId()
        {
            var pageTypeId = Guid.NewGuid();
            var page = new Page(Guid.NewGuid(), "T", "/t", "d", pageTypeId, null, false);
            page.AddContent(Guid.NewGuid(), page.Id, "v", 0);

            var list = new[] { page };

            var spec = new PagesWithContentThatImplementPageTypeSpecification(pageTypeId);

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
            Assert.Equal(pageTypeId, results[0].PageTypeId);
        }
    }
}