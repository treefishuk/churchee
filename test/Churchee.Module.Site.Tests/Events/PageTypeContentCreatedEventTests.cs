using Churchee.Module.Site.Events;

namespace Churchee.Module.Site.Tests.Events
{
    public class PageTypeContentCreatedEventTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var contentId = Guid.NewGuid();
            var typeId = Guid.NewGuid();

            // Act
            var evt = new PageTypeContentCreatedEvent(tenantId, contentId, typeId);

            // Assert
            Assert.Equal(tenantId, evt.ApplicationTenantId);
            Assert.Equal(contentId, evt.PageTypeContentId);
            Assert.Equal(typeId, evt.PageTypeId);
        }
    }
}