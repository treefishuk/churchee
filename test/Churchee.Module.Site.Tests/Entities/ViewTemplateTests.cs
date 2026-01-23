using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Tests.Entities
{
    public class ViewTemplateTests
    {
        private static Guid TenantId => Guid.Parse("41c3d2f6-6189-401e-8037-66a67416f94d");

        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            string location = "/Views/Home/Index.cshtml";
            string content = "<h1>Hello</h1>";

            // Act
            var template = new ViewTemplate(TenantId, location, content);

            // Assert
            Assert.Equal(TenantId, template.ApplicationTenantId);
            Assert.Equal(location, template.Location);
            Assert.Equal(content, template.Content);
            Assert.Null(template.TenantLocation);
        }

        [Fact]
        public void SetContent_UpdatesContent()
        {
            // Arrange
            var template = new ViewTemplate(TenantId, "/Views/Home/Index.cshtml", "old content");
            string newContent = "<h2>New Content</h2>";

            // Act
            template.SetContent(newContent);

            // Assert
            Assert.Equal(newContent, template.Content);
        }
    }
}