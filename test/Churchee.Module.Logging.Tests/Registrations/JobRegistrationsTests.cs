using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Logging.Jobs;
using Churchee.Module.Logging.Registrations;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Logging.Tests.Registrations
{
    public class JobRegistrationsTests
    {
        [Fact]
        public void JobRegistrations_Should_Create_Jobs()
        {
            // Arrange
            var mockJobService = new Mock<IJobService>();
            var services = new ServiceCollection();
            services.AddSingleton(mockJobService.Object);

            var serviceProvider = services.BuildServiceProvider();

            var cut = new JobRegistrations();

            // Act
            cut.Execute(services, serviceProvider);

            // Assert
            cut.Priority.Should().Be(6000);
            mockJobService.Verify(j => j.ScheduleJob(It.IsAny<string>(), It.IsAny<Expression<Func<ClearLogsJob, Task>>>(), It.IsAny<Func<string>>()), Times.Once);
            mockJobService.Verify(j => j.QueueJob(It.IsAny<Expression<Func<ErrorLoggingTestJob, Task>>>()), Times.Once);
        }

    }
}
