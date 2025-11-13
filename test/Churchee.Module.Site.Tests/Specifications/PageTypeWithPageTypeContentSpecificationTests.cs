using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageTypeWithPageTypeContentSpecificationTests
    {
        [Fact]
        public void Includes_PageTypeContent_And_Filters_By_Id()
        {
            var pageTypeId = Guid.NewGuid();
            var pt = new PageType(pageTypeId, Guid.NewGuid(), Guid.NewGuid(), true, "PT", false);
            pt.AddPageTypeContent(Guid.NewGuid(), "Name", "text", false, 1);

            var list = new[] { pt };

            var spec = new PageTypeWithPageTypeContentSpecification(pageTypeId);

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
            Assert.Equal(pageTypeId, results[0].Id);
        }
    }
}
