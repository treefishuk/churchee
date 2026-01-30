using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Site.Jobs;
using Churchee.Module.Site.Registration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq.Expressions;

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
            int priority = registrations.Priority;

            // Assert
            Assert.Equal(6000, priority);
        }

        [Fact]
        public void Execute_RegistersPublishArticlesJobAndSchedulesIt()
        {
            // Arrange
            var services = new ServiceCollection();
            var jobServiceMock = new Mock<IJobService>();

            // Setup serviceProvider to return jobServiceMock when requested
            services.AddScoped(sp => jobServiceMock.Object);

            var registrations = new JobRegistrations();

            var serviceProvider = services.BuildServiceProvider();

            // Act
            registrations.Execute(services, serviceProvider);

            // Assert

            // Check that PublishArticlesJob is registered
            var descriptor = Assert.Single(services, d => d.ServiceType == typeof(PublishArticlesJob));

            // Check that PublishArticlesJob is registered as Scoped
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);

            // ScheduleJob should be called
            jobServiceMock.Verify(js =>
                js.ScheduleJob(
                    "PublishArticles",
                    It.IsAny<Expression<Func<PublishArticlesJob, Task>>>(),
                    () => Hangfire.Cron.Daily(1)),
                Times.Once);
        }
    }
}