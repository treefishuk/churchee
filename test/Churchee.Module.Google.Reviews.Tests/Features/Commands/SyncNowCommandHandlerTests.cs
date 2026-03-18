using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Google.Reviews.Features.Commands;
using Churchee.Module.Google.Reviews.Jobs;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Google.Reviews.Tests.Features.Commands
{
    public class SyncNowCommandHandlerTests
    {

        [Fact]
        public async Task Handle_Returns_Successful_CommandResponse()
        {
            // Arrange
            var mockJobService = new Mock<IJobService>();
            var mockCurrentUser = new Mock<ICurrentUser>();
            var mockDistributedCache = new Mock<IDistributedCache>();

            var handler = new SyncNowCommandHandler(mockJobService.Object, mockCurrentUser.Object, mockDistributedCache.Object);
            var command = new SyncNowCommand();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_SchedulesJob()
        {
            // Arrange
            var mockJobService = new Mock<IJobService>();
            var mockCurrentUser = new Mock<ICurrentUser>();
            var mockDistributedCache = new Mock<IDistributedCache>();
            var handler = new SyncNowCommandHandler(mockJobService.Object, mockCurrentUser.Object, mockDistributedCache.Object);
            var command = new SyncNowCommand();

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            mockJobService.Verify(x => x.QueueJob(It.IsAny<Expression<Func<GoogleReviewsSyncJob, Task>>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CachesSyncNowRequest()
        {
            // Arrange
            var mockJobService = new Mock<IJobService>();
            var mockCurrentUser = new Mock<ICurrentUser>();
            var mockDistributedCache = new Mock<IDistributedCache>();
            var handler = new SyncNowCommandHandler(mockJobService.Object, mockCurrentUser.Object, mockDistributedCache.Object);
            var command = new SyncNowCommand();

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            mockDistributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_QueueJob_Not_Called_If_SyncNow_Request_Cached()
        {
            // Arrange
            var mockJobService = new Mock<IJobService>();
            var mockCurrentUser = new Mock<ICurrentUser>();
            var mockDistributedCache = new Mock<IDistributedCache>();
            var handler = new SyncNowCommandHandler(mockJobService.Object, mockCurrentUser.Object, mockDistributedCache.Object);
            var command = new SyncNowCommand();

            // Simulate existing cache entry
            mockDistributedCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new byte[1]);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            mockJobService.Verify(x => x.QueueJob(It.IsAny<Expression<Func<Task>>>()), Times.Never);
        }
    }
}