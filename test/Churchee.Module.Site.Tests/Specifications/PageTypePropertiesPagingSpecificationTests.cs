using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageTypePropertiesPagingSpecificationTests
    {
        [Fact]
        public void Constructor_Sets_Query_And_Paging()
        {
            var pageTypeSystemId = Guid.NewGuid();
            var pageType1Id = Guid.NewGuid();
            var pageType2Id = Guid.NewGuid();

            var p1 = new PageTypeProperty
            {
                Name = "Alpha",
                PageType = new PageType(pageType1Id, pageTypeSystemId, Guid.NewGuid(), true, "PT", false)
            };
            var p2 = new PageTypeProperty
            {
                Name = "Beta",
                PageType = new PageType(pageType2Id, pageTypeSystemId, Guid.NewGuid(), true, "PT2", false)
            };

            var list = new[] { p2, p1 };

            var spec = new PageTypePropertiesPagingSpecification(pageType1Id, null, 0, 0, "NAME", "ASC");

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
        }
    }
}
