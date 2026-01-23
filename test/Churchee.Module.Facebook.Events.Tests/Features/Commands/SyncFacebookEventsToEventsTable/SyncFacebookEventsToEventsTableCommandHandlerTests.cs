using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Facebook.Events.Features.Commands;
using Churchee.Module.Facebook.Events.Features.Commands.SyncFacebookEventsToEventsTable;
using Churchee.Module.Facebook.Events.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Facebook.Events.Tests.Features.Commands.SyncFacebookEventsToEventsTable
{
    public class SyncFacebookEventsToEventsTableCommandHandlerTests
    {

        private readonly Mock<ICurrentUser> _currentUser = new();
        private readonly Mock<IJobService> _jobService = new();
        private readonly Mock<ILogger<SyncFacebookEventsToEventsTableCommandHandler>> _logger = new();

        private SyncFacebookEventsToEventsTableCommandHandler CreateHandler()
        {
            return new SyncFacebookEventsToEventsTableCommandHandler(
                _currentUser.Object,
                _logger.Object,
                _jobService.Object
            );
        }

        [Fact]
        public async Task Handle_SchedulesAndQueuesJobs_ReturnsSuccess()
        {
            // Arrange
            var handler = CreateHandler();
            var command = new SyncFacebookEventsToEventsTableCommand();
            var tenantId = Guid.NewGuid();
            _currentUser.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _jobService.Verify(x => x.ScheduleJob(
                $"{tenantId}_FacebookEvents",
                It.IsAny<Expression<Func<SyncFacebookEventsJob, Task>>>(),
                It.IsAny<Func<string>>()), Times.Once);

            _jobService.Verify(x => x.QueueJob(It.IsAny<Expression<Func<SyncFacebookEventsJob, Task>>>()), Times.Once);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Handle_Exception_LogsErrorAndReturnsErrorResponse()
        {
            // Arrange
            var handler = CreateHandler();
            var command = new SyncFacebookEventsToEventsTableCommand();
            _currentUser.Setup(x => x.GetApplicationTenantId()).ThrowsAsync(new Exception("fail"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString() == "Error syncing Facebook events" && t.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal("Failed To Sync", result.Errors[0].Description);
        }



    }
}
