using Churchee.Module.Tenancy.Events;

namespace Churchee.Module.Tenancy.Tests.Events
{
    public class TenantAddedEventTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act
            var cut = new TenantAddedEvent(tenantId);

            // Assert
            Assert.Equal(tenantId, cut.ApplicationTenantId);
        }
    }
}
