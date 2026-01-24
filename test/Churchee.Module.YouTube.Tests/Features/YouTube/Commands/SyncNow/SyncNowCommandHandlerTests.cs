using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Module.YouTube.Features.YouTube.Commands.SyncNow;
using Churchee.Module.YouTube.Jobs;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Linq.Expressions;
using System.Text;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Commands.SyncNow
{
    public class SyncNowCommandHandlerTests
    {
        private readonly Mock<IJobService> _mockJobService;
        private readonly Mock<IDistributedCache> _mockDistributedCache;
        private readonly Mock<ICurrentUser> _mockCurrentUser;

        public SyncNowCommandHandlerTests()
        {
            _mockJobService = new Mock<IJobService>();
            _mockDistributedCache = new Mock<IDistributedCache>();
            _mockCurrentUser = new Mock<ICurrentUser>();
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenAlreadyRunning()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();

            _mockCurrentUser.Setup(s => s.GetApplicationTenantId()).ReturnsAsync(applicationTenantId);

            _mockDistributedCache.Setup(s => s.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes("1"));

            var command = new SyncNowCommand();

            var cut = new SyncNowCommandHandler(_mockJobService.Object, _mockCurrentUser.Object, _mockDistributedCache.Object);

            // Act
            var response = await cut.Handle(command, CancellationToken.None);

            // Assert
            response.Errors.Count.Should().Be(1);

            _mockJobService.Verify(x => x.QueueJob(It.IsAny<Expression<Func<SyncYouTubeVideosJob, Task>>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QueuesJob_WhenNotAlreadyRunning()
        {
            // Arrange
            var appTenanatId = Guid.NewGuid();

            _mockCurrentUser.Setup(s => s.GetApplicationTenantId()).ReturnsAsync(appTenanatId);

            _mockDistributedCache.Setup(s => s.GetAsync($"YouTubeSyncJobQueued_{appTenanatId}", It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes(""));

            var command = new SyncNowCommand();

            var cut = new SyncNowCommandHandler(_mockJobService.Object, _mockCurrentUser.Object, _mockDistributedCache.Object);

            // Act
            var response = await cut.Handle(command, CancellationToken.None);

            // Assert
            response.Errors.Count.Should().Be(0);

            _mockJobService.Verify(x => x.QueueJob(It.IsAny<Expression<Func<SyncYouTubeVideosJob, Task>>>()), Times.Once);
        }

    }
}
