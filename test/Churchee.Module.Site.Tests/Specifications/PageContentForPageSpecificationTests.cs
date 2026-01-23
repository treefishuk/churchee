using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageContentForPageSpecificationTests
    {
        [Fact]
        public void Evaluates_Filter_And_Order_By_PageTypeContent_Order()
        {
            var pageId = Guid.NewGuid();

            var content1 = new PageContent(Guid.NewGuid(), pageId, "v1", 0)
            {
                PageTypeContent = new PageTypeContent(Guid.NewGuid(), Guid.NewGuid(), "t", false, "A", 2)
            };
            var content2 = new PageContent(Guid.NewGuid(), pageId, "v2", 0)
            {
                PageTypeContent = new PageTypeContent(Guid.NewGuid(), Guid.NewGuid(), "t", false, "B", 1)
            };

            var other = new PageContent(Guid.NewGuid(), Guid.NewGuid(), "x", 0)
            {
                PageTypeContent = new PageTypeContent(Guid.NewGuid(), Guid.NewGuid(), "t", false, "C", 3)
            };

            var list = new[] { content1, content2, other };

            var spec = new PageContentForPageSpecification(pageId);

            var results = spec.Evaluate(list).ToList();

            Assert.Equal(2, results.Count);
            // ensure ordering by PageTypeContent.Order ascending
            Assert.Equal(1, results[0].PageTypeContent.Order);
            Assert.Equal(2, results[1].PageTypeContent.Order);
            // ensure the values correspond
            Assert.Equal("v2", results[0].Value);
            Assert.Equal("v1", results[1].Value);
        }
    }
}
