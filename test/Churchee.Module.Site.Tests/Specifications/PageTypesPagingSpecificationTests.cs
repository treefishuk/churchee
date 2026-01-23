using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageTypesPagingSpecificationTests
    {
        [Fact]
        public void Constructor_Sets_Query_With_Search_Ordering_And_Paging()
        {
            var pt1 = new PageType(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), true, "Alpha", false);
            var pt2 = new PageType(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), true, "Beta", false);

            var list = new[] { pt2, pt1 };

            var spec = new PageTypesPagingSpecification("Al", 10, 0, "NAME", "ASC");

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
        }
    }
}