using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageWithContentAndPropertiesSpecificationTests
    {
        [Fact]
        public void Includes_Content_And_Properties_And_Filters_By_PageId()
        {
            _ = Guid.NewGuid();
            var page = new Page(Guid.NewGuid(), "T", "/t", "d", Guid.NewGuid(), null, false);
            page.AddContent(Guid.NewGuid(), page.Id, "v1", 0);

            var list = new[] { page };

            var spec = new PageWithContentAndPropertiesSpecification(page.Id);

            var results = spec.Evaluate(list).ToList();

            _ = Assert.Single(results);
            Assert.Equal(page.Id, results[0].Id);
        }
    }
}
