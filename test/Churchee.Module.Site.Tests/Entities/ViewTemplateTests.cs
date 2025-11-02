using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Tests.Entities
{
    public class ViewTemplateTests
    {
        private static Guid TenantId => Guid.NewGuid();

        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            var location = "/Views/Home/Index.cshtml";
            var content = "<h1>Hello</h1>";

            // Act
            var template = new ViewTemplate(TenantId, location, content);

            // Assert
            Assert.Equal(TenantId, template.ApplicationTenantId);
            Assert.Equal(location, template.Location);
            Assert.Equal(content, template.Content);
            Assert.Null(template.TenantLocation);
            Assert.Equal(default(DateTime), template.LastRequested);
        }

        [Fact]
        public void SetContent_UpdatesContent()
        {
            // Arrange
            var template = new ViewTemplate(TenantId, "/Views/Home/Index.cshtml", "old content");
            var newContent = "<h2>New Content</h2>";

            // Act
            template.SetContent(newContent);

            // Assert
            Assert.Equal(newContent, template.Content);
        }
    }
}