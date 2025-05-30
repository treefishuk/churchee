using Churchee.Module.Tenancy.Entities;

namespace Churchee.Module.Tenancy.Tests.Entities
{
    public class ApplicationHostTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            string host = "test.example.com";
            var tenantId = Guid.NewGuid();

            // Act
            var applicationHost = new ApplicationHost(host, tenantId);

            // Assert
            Assert.Equal(host, applicationHost.Host);
            Assert.Equal(tenantId, applicationHost.ApplicationTenantId);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenHostIsNull()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ApplicationHost(null, tenantId));
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenHostIsEmpty()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ApplicationHost(string.Empty, tenantId));
        }

        [Fact]
        public void DefaultConstructor_ShouldInitializeHostToEmptyString()
        {
            // Act
            var applicationHost = (ApplicationHost?)Activator.CreateInstance(typeof(ApplicationHost), nonPublic: true);

            // Assert
            Assert.NotNull(applicationHost);
            Assert.Equal(string.Empty, applicationHost.Host);
        }
    }
}