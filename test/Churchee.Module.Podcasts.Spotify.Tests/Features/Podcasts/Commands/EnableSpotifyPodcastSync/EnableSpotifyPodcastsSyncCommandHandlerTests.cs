using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands;
using FluentAssertions;
using Hangfire;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Podcasts.Spotify.Tests.Features.Podcasts.Commands.EnableSpotifyPodcastSync
{

    public class EnableSpotifyPodcastsSyncCommandHandlerTests
    {
        private readonly Mock<ISettingStore> _settingStoreMock;
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly Mock<IBlobStore> _blobStoreMock;
        private readonly Mock<IImageProcessor> _IImageProcessorMock;
        private readonly Mock<IJobService> _jobServiceMock;

        private readonly EnableSpotifyPodcastsSyncCommandHandler _handler;

        public EnableSpotifyPodcastsSyncCommandHandlerTests()
        {
            _settingStoreMock = new Mock<ISettingStore>();
            _dataStoreMock = new Mock<IDataStore>();
            _currentUserMock = new Mock<ICurrentUser>();
            _blobStoreMock = new Mock<IBlobStore>();
            _IImageProcessorMock = new Mock<IImageProcessor>();
            _jobServiceMock = new Mock<IJobService>();

            _handler = new EnableSpotifyPodcastsSyncCommandHandler(
                _settingStoreMock.Object,
                _currentUserMock.Object,
                _dataStoreMock.Object,
                _blobStoreMock.Object,
                _IImageProcessorMock.Object,
                _jobServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldSetSetting()
        {
            // Arrange
            string rssFeed = "http://example.com/rss";
            var command = new EnableSpotifyPodcastSyncCommand(rssFeed);
            var cancellationToken = CancellationToken.None;

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _settingStoreMock.Verify(x => x.AddOrUpdateSetting(It.IsAny<Guid>(), It.IsAny<Guid>(), "SpotifyRSSFeedUrl", command.SpotifyFMRSSFeed), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldAddOrUpdateRecurringJob()
        {
            // Arrange
            string rssFeed = "http://example.com/rss";
            var tenantId = Guid.NewGuid();
            var command = new EnableSpotifyPodcastSyncCommand(rssFeed);
            var cancellationToken = CancellationToken.None;
            _currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _jobServiceMock.Verify(x => x.ScheduleJob($"{tenantId}_SpotifyPodcasts", It.IsAny<Expression<Func<Task>>>(), Cron.Daily), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnCommandResponse()
        {
            // Arrange

            //arrange
            string rssFeed = "http://example.com/rss";
            var command = new EnableSpotifyPodcastSyncCommand(rssFeed);

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().BeOfType<CommandResponse>();
        }
    }
}
