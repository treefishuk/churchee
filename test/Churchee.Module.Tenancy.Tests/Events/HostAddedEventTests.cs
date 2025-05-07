using Churchee.Module.Tenancy.Events;

namespace Churchee.Module.Tenancy.Tests.Events
{
    public class HostAddedEventTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var host = "test.example.com";
            var tenantId = Guid.NewGuid();

            // Act
            var cut = new HostAddedEvent(host, tenantId);

            // Assert
            Assert.Equal(host, cut.Host);
            Assert.Equal(tenantId, cut.ApplicationTenantId);
        }
    }
}
