using Churchee.Module.Tenancy.Entities;
using MediatR;
using Moq;

namespace Churchee.Module.Tenancy.Tests.Entities
{
    public class ApplicationTenantTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var name = "Test Tenant";
            var charityNumber = 12345;

            // Act
            var tenant = new ApplicationTenant(tenantId, name, charityNumber);

            // Assert
            Assert.Equal(tenantId, tenant.Id);
            Assert.Equal(name, tenant.Name);
            Assert.Equal(name.ToCamelCase(), tenant.DevName);
            Assert.Equal(charityNumber, tenant.CharityNumber);
            Assert.Empty(tenant.Hosts);
            Assert.Empty(tenant.Features);
            Assert.Empty(tenant.DomainEvents);
        }

        [Fact]
        public void SetName_ShouldUpdateName()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Old Name", 12345);
            var newName = "New Name";

            // Act
            tenant.SetName(newName);

            // Assert
            Assert.Equal(newName, tenant.Name);
        }

        [Fact]
        public void SetName_ShouldThrowException_WhenNameIsNull()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Old Name", 12345);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => tenant.SetName(null));
        }

        [Fact]
        public void AddHost_ShouldAddNewHost()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);
            var hostname = "test.example.com";

            // Act
            tenant.AddHost(hostname);

            // Assert
            Assert.Single(tenant.Hosts);
            Assert.Equal(hostname, tenant.Hosts.First().Host);
        }

        [Fact]
        public void AddHost_ShouldThrowException_WhenHostnameIsNull()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => tenant.AddHost(null));
        }

        [Fact]
        public void RemoveHost_ShouldRemoveHost_WhenHostExists()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);
            var hostname = "test.example.com";
            tenant.AddHost(hostname);
            var hostId = tenant.Hosts.First().Id;

            // Act
            tenant.RemoveHost(hostId);

            // Assert
            Assert.Empty(tenant.Hosts);
        }

        [Fact]
        public void RemoveHost_ShouldDoNothing_WhenHostDoesNotExist()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);

            // Act
            tenant.RemoveHost(Guid.NewGuid());

            // Assert
            Assert.Empty(tenant.Hosts);
        }

        [Fact]
        public void AddFeature_ShouldAddNewFeature()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);
            var featureName = "Test Feature";

            // Act
            tenant.AddFeature(featureName);

            // Assert
            Assert.Single(tenant.Features);
            Assert.Equal(featureName, tenant.Features.First().Name);
        }

        [Fact]
        public void AddFeature_ShouldThrowException_WhenFeatureNameIsNull()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => tenant.AddFeature(null));
        }

        [Fact]
        public void RemoveFeature_ShouldRemoveFeature_WhenFeatureExists()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);
            var featureName = "Test Feature";
            tenant.AddFeature(featureName);
            var featureId = tenant.Features.First().Id;

            // Act
            tenant.RemoveFeature(featureId);

            // Assert
            Assert.Empty(tenant.Features);
        }

        [Fact]
        public void RemoveFeature_ShouldDoNothing_WhenFeatureDoesNotExist()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);

            // Act
            tenant.RemoveFeature(Guid.NewGuid());

            // Assert
            Assert.Empty(tenant.Features);
        }

        [Fact]
        public void AddDomainEvent_ShouldAddEvent()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);
            var domainEvent = new Mock<INotification>().Object;

            // Act
            tenant.AddDomainEvent(domainEvent);

            // Assert
            Assert.Single(tenant.DomainEvents);
            Assert.Contains(domainEvent, tenant.DomainEvents);
        }

        [Fact]
        public void RemoveDomainEvent_ShouldRemoveEvent()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);
            var domainEvent = new Mock<INotification>().Object;
            tenant.AddDomainEvent(domainEvent);

            // Act
            tenant.RemoveDomainEvent(domainEvent);

            // Assert
            Assert.Empty(tenant.DomainEvents);
        }

        [Fact]
        public void ClearDomainEvents_ShouldRemoveAllEvents()
        {
            // Arrange
            var tenant = new ApplicationTenant(Guid.NewGuid(), "Test Tenant", 12345);
            tenant.AddDomainEvent(new Mock<INotification>().Object);
            tenant.AddDomainEvent(new Mock<INotification>().Object);

            // Act
            tenant.ClearDomainEvents();

            // Assert
            Assert.Empty(tenant.DomainEvents);
        }
    }
}
