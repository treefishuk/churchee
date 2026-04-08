using Churchee.Module.ChurchSuite.Jobs;
using Churchee.Module.ChurchSuite.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.ChurchSuite.Tests.Registrations
{
    public class ServiceRegistrationsTests
    {
        [Fact]
        public void SyncChurchSuiteEventsJob_Registered()
        {
            // Arrange
            var services = new ServiceCollection();

            var serviceProvider = services.BuildServiceProvider();

            var serviceRegistrations = new ServiceRegistrations();

            // Act
            serviceRegistrations.Execute(services, serviceProvider);

            serviceProvider = services.BuildServiceProvider();

            // Assert
            var descriptor = Assert.Single(services, d => d.ServiceType == typeof(SyncChurchSuiteEventsJob));

        }

        [Fact]
        public void Priority_ShouldBe200()
        {
            // Arrange
            var serviceRegistrations = new ServiceRegistrations();

            // Act & Assert
            Assert.Equal(6000, serviceRegistrations.Priority);
        }
    }
}
