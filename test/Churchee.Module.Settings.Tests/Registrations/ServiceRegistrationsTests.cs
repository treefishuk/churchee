using Churchee.Common.Storage;
using Churchee.Module.Settings.Registrations;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.Settings.Tests.Registrations
{
    public class ServiceRegistrationsTests
    {

        [Fact]
        public void ServiceRegistrations_Priority_Should_Be_5000()
        {
            // Arrange
            var serviceRegistrations = new ServiceRegistrations();

            // Act
            var priority = serviceRegistrations.Priority;

            // Assert
            priority.Should().Be(5000);
        }

        [Fact]
        public void ServiceRegistrations_Should_Register_ISettingStore_As_Scoped()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceRegistrations = new ServiceRegistrations();

            // Act
            serviceRegistrations.Execute(serviceCollection, serviceProvider);
            var serviceDescriptor = serviceCollection.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(ISettingStore));

            // Assert
            serviceDescriptor.Should().NotBeNull();
            serviceDescriptor?.Lifetime.Should().Be(ServiceLifetime.Scoped);
        }
    }
}
