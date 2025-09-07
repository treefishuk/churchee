using Churchee.Module.Site.Entities;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Site.Tests.Entities
{
    public class PageTypePropertyTests
    {
        [Fact]
        public void DevName_Should_ReturnCorrectDevName()
        {
            // Arrange
            var pageTypeProperty = new PageTypeProperty
            {
                Name = "Sample Name"
            };

            // Act
            var devName = pageTypeProperty.DevName;

            // Assert
            devName.Should().Be("sampleName");
        }

        [Fact]
        public void PageTypeProperty_Should_HaveCorrectProperties()
        {
            // Arrange
            var pageType = new PageType(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), true, "Sample PageType");
            var pageTypeProperty = new PageTypeProperty
            {
                PageType = pageType,
                Name = "Sample Property",
                Type = "string",
                Required = true
            };

            // Act & Assert
            pageTypeProperty.PageType.Should().Be(pageType);
            pageTypeProperty.Name.Should().Be("Sample Property");
            pageTypeProperty.Type.Should().Be("string");
            pageTypeProperty.Required.Should().BeTrue();
        }
    }
}