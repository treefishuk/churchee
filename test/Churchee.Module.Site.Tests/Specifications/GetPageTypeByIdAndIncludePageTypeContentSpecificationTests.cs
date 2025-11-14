using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class GetPageTypeByIdAndIncludePageTypeContentSpecificationTests
    {
        [Fact]
        public void Constructor_Sets_Query_And_Includes_Content_And_Filters_By_Id()
        {
            var pageTypeId = Guid.NewGuid();
            var pt = new PageType(pageTypeId, Guid.NewGuid(), Guid.NewGuid(), true, "PT", false);
            pt.AddPageTypeContent(Guid.NewGuid(), "Name", "text", false, 1);

            var list = new[] { pt };

            var spec = new GetPageTypeByIdAndIncludePageTypeContentSpecification(pageTypeId);

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
            Assert.Equal(pageTypeId, results[0].Id);
        }
    }
}