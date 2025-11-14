using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageTypeContentPagingSpecificationTests
    {
        [Fact]
        public void Constructor_Applies_Search_Ordering_And_Paging()
        {
            var pageType = new PageType(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), true, "PT", false);
            pageType.AddPageTypeContent(Guid.NewGuid(), "Alpha", "text", false, 1);
            pageType.AddPageTypeContent(Guid.NewGuid(), "Beta", "text", false, 2);

            var list = pageType.PageTypeContent.ToArray();

            var spec = new PageTypeContentPagingSpecification(pageType.Id, "Al", 10, 0, "NAME", "ASC");

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
        }
    }
}