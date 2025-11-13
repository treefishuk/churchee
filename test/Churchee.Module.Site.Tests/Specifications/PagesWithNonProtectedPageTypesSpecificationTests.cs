using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PagesWithNonProtectedPageTypesSpecificationTests
    {
        [Fact]
        public void Constructor_Sets_Query_With_Optional_CurrentPageId()
        {
            var tenant = Guid.NewGuid();
            var currentPageId = Guid.NewGuid();

            var pageType = new PageType(Guid.NewGuid(), Guid.NewGuid(), tenant, true, "PT", false);

            var page1 = new Page(tenant, "A", "/a", "d", pageType.Id, null, false)
            {
                PageType = pageType
            };
            var page2 = new Page(tenant, "B", "/b", "d", pageType.Id, null, false)
            {
                PageType = pageType,
                Url = "/" // root should be excluded
            };

            var list = new[] { page1, page2 };

            var spec = new PagesWithNonProtectedPageTypesSpecification(tenant, currentPageId);

            var results = spec.Evaluate(list).ToList();

            Assert.All(results, r => Assert.Equal(tenant, r.ApplicationTenantId));
        }
    }
}
