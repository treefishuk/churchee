using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Entities;
using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands;
using Churchee.Test.Helpers.Validation;
using Hangfire;
using Moq;

namespace Churchee.Module.Podcasts.Spotify.Tests.Features.Podcasts.Commands
{

    public class DisableSpotifyPodcastSyncCommandHandlerTests
    {
        private readonly Mock<ISettingStore> _settingStoreMock;
        private readonly Mock<IRecurringJobManager> _recurringJobManagerMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly DisableSpotifyPodcastSyncCommandHandler _handler;
        private readonly Mock<IRepository<Podcast>> _repositoryMock;



        public DisableSpotifyPodcastSyncCommandHandlerTests()
        {
            _settingStoreMock = new Mock<ISettingStore>();
            _recurringJobManagerMock = new Mock<IRecurringJobManager>();
            _currentUserMock = new Mock<ICurrentUser>();
            _repositoryMock = new Mock<IRepository<Podcast>>();
            _dataStoreMock = new Mock<IDataStore>();
            _dataStoreMock.Setup(ds => ds.GetRepository<Podcast>()).Returns(_repositoryMock.Object);

            _handler = new DisableSpotifyPodcastSyncCommandHandler(
                _settingStoreMock.Object,
                _currentUserMock.Object,
                _recurringJobManagerMock.Object,
                _dataStoreMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldClearSetting()
        {
            // Arrange
            var command = new DisableSpotifyPodcastSyncCommand();
            var cancellationToken = CancellationToken.None;

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _settingStoreMock.Verify(x => x.ClearSetting(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldRemoveRecurringJob()
        {
            // Arrange
            var command = new DisableSpotifyPodcastSyncCommand();
            var cancellationToken = CancellationToken.None;
            var tenantId = Guid.NewGuid();

            _currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _recurringJobManagerMock.Verify(x => x.RemoveIfExists($"{tenantId}_SpotifyPodcasts"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldDeleteSpotifyPodcasts()
        {
            // Arrange
            var command = new DisableSpotifyPodcastSyncCommand();
            var cancellationToken = CancellationToken.None;

            var testEntity = new Podcast(applicationTenantId: Guid.NewGuid(), audioUri: string.Empty, publishedDate: DateTime.Now, sourceName: "Spotify", sourceId: string.Empty, title: "test", description: "test", imageUrl: string.Empty, thumbnailUrl: string.Empty, podcastsUrl: string.Empty, podcastDetailPageTypeId: Guid.NewGuid());

            var spotifyPodcasts = new[] { testEntity }.AsQueryable();

            _repositoryMock.Setup(x => x.GetQueryable()).Returns(spotifyPodcasts);
            _dataStoreMock.Setup(x => x.GetRepository<Podcast>()).Returns(_repositoryMock.Object);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _repositoryMock.Verify(x => x.PermanentDelete(It.IsAny<Podcast>()), Times.Once);
            _dataStoreMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnCommandResponse()
        {
            // Arrange
            var command = new DisableSpotifyPodcastSyncCommand();
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().BeOfType<CommandResponse>();
        }
    }
}