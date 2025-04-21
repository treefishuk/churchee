using Churchee.Module.Tenancy.Entities;

namespace Churchee.Module.Tenancy.Tests.Entities
{
    public class ApplicationFeatureTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var featureName = "Test Feature";
            var tenantId = Guid.NewGuid();

            // Act
            var applicationFeature = new ApplicationFeature(featureName, tenantId);

            // Assert
            Assert.Equal(featureName, applicationFeature.Name);
            Assert.Equal(tenantId, applicationFeature.ApplicationTenantId);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenNameIsNull()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ApplicationFeature(null, tenantId));
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenNameIsEmpty()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ApplicationFeature(string.Empty, tenantId));
        }

        [Fact]
        public void DefaultConstructor_ShouldInitializeNameToEmptyString()
        {
            // Act
            var applicationFeature = (ApplicationFeature)Activator.CreateInstance(typeof(ApplicationFeature), nonPublic: true);

            // Assert
            Assert.NotNull(applicationFeature);
            Assert.Equal(string.Empty, applicationFeature.Name);
        }
    }
}
