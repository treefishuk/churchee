using Churchee.Module.Site.Events;

namespace Churchee.Module.Site.Tests.Events
{
    public class PageTypeCreatedEventTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var name = "Test Page Type";

            // Act
            var evt = new PageTypeCreatedEvent(tenantId, name);

            // Assert
            Assert.Equal(tenantId, evt.ApplicationTenantId);
            Assert.Equal(name, evt.Name);
        }
    }
}