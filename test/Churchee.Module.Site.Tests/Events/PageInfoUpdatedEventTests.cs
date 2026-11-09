using Churchee.Module.Site.Events;

namespace Churchee.Module.Site.Tests.Events
{
    public class PageInfoUpdatedEventTests
    {
        [Fact]
        public void Constructor_SetsPageIdCorrectly()
        {
            // Arrange
            var pageId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();

            // Act
            var evt = new PageInfoUpdatedEvent(tenantId, pageId);

            // Assert
            Assert.Equal(tenantId, evt.ApplicationTenantId);
            Assert.Equal(pageId, evt.PageId);
        }
    }
}