using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Site.Jobs;
using Churchee.Module.Site.Registration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Churchee.Module.Site.Tests.Registrations
{
    public class JobRegistrationsTests
    {
        [Fact]
        public void Priority_ReturnsExpectedValue()
        {
            // Arrange
            var registrations = new JobRegistrations();

            // Act
            var priority = registrations.Priority;

            // Assert
            Assert.Equal(6000, priority);
        }

        [Fact]
        public void Execute_RegistersPublishArticlesJobAndSchedulesIt()
        {
            // Arrange
            var services = new ServiceCollection();
            var jobServiceMock = new Mock<IJobService>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            // Setup serviceProvider to return jobServiceMock when requested
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IJobService)))
                .Returns(jobServiceMock.Object);

            serviceProviderMock
                .Setup(sp => sp.GetRequiredService(typeof(IJobService)))
                .Returns(jobServiceMock.Object);

            var registrations = new JobRegistrations();

            // Act
            registrations.Execute(services, serviceProviderMock.Object);

            // Assert: PublishArticlesJob should be registered as Scoped
            var descriptor = Assert.Single(services, d => d.ServiceType == typeof(PublishArticlesJob));

            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }
}