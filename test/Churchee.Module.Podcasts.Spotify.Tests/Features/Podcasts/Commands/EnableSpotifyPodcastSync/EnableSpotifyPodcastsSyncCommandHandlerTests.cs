using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Entities;
using Churchee.Module.Podcasts.Specifications;
using Churchee.Module.Podcasts.Spotify.Exceptions;
using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands;
using Churchee.Module.Podcasts.Spotify.Specifications;
using Churchee.Module.Site.Entities;
using Churchee.Test.Helpers.Validation;
using Hangfire;
using Moq;
using Moq.Protected;
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
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<IRepository<Podcast>> _podcastRepositoryMock;
        private readonly Mock<IRepository<PageType>> _pageTypeRepositoryMock;

        private readonly EnableSpotifyPodcastsSyncCommandHandler _handler;

        public EnableSpotifyPodcastsSyncCommandHandlerTests()
        {
            _settingStoreMock = new Mock<ISettingStore>();
            _dataStoreMock = new Mock<IDataStore>();
            _currentUserMock = new Mock<ICurrentUser>();
            _blobStoreMock = new Mock<IBlobStore>();
            _IImageProcessorMock = new Mock<IImageProcessor>();
            _jobServiceMock = new Mock<IJobService>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _podcastRepositoryMock = new Mock<IRepository<Podcast>>();
            _pageTypeRepositoryMock = new Mock<IRepository<PageType>>();
            _dataStoreMock.Setup(ds => ds.GetRepository<Podcast>()).Returns(_podcastRepositoryMock.Object);
            _dataStoreMock.Setup(ds => ds.GetRepository<PageType>()).Returns(_pageTypeRepositoryMock.Object);

            var httpClientFactory = new TestHttpClientFactory(_mockHttpMessageHandler);

            _handler = new EnableSpotifyPodcastsSyncCommandHandler(
                _settingStoreMock.Object,
                _currentUserMock.Object,
                _dataStoreMock.Object,
                _blobStoreMock.Object,
                _IImageProcessorMock.Object,
                _jobServiceMock.Object,
                httpClientFactory
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
            _jobServiceMock.Verify(x => x.ScheduleJob($"{tenantId}_SpotifyPodcasts", It.IsAny<Expression<Func<Task>>>(), Cron.Hourly), Times.Once);
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


        [Fact]
        public async Task SyncPodcasts_WhenItemsAlreadyExist_AddNoNewItems()
        {
            // arrange
            string rssFeed = "http://example.com/rss";
            var tenantId = Guid.NewGuid();
            var command = new EnableSpotifyPodcastSyncCommand(rssFeed);
            var cancellationToken = CancellationToken.None;

            ConfigureTestXml();

            _podcastRepositoryMock.Setup(s => s.AnyWithFiltersDisabled(It.IsAny<Expression<Func<Podcast, bool>>>())).Returns(true);

            // act
            await _handler.SyncPodcasts(command, tenantId, rssFeed, CancellationToken.None);

            // Assert
            _podcastRepositoryMock.Verify(x => x.AddRange(It.IsAny<List<Podcast>>()), Times.Never);
            _dataStoreMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SyncPodcasts_WhenItemsItemsDoNotExist_AddRangeCalled()
        {
            // arrange
            string rssFeed = "http://example.com/rss";
            var tenantId = Guid.NewGuid();
            var command = new EnableSpotifyPodcastSyncCommand(rssFeed);
            var cancellationToken = CancellationToken.None;

            ConfigureTestXml();

            _podcastRepositoryMock.Setup(s => s.AnyWithFiltersDisabled(It.IsAny<Expression<Func<Podcast, bool>>>())).Returns(false);
            _podcastRepositoryMock.Setup(s => s.FirstOrDefaultAsync(It.IsAny<PodcastByAudioUrlSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Podcast());
            _pageTypeRepositoryMock.Setup(s => s.FirstOrDefaultAsync(It.IsAny<PageTypeFromSystemKeySpecification>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid());

            // act
            await _handler.SyncPodcasts(command, tenantId, rssFeed, CancellationToken.None);

            // Assert
            _podcastRepositoryMock.Verify(x => x.AddRange(It.IsAny<List<Podcast>>()), Times.Once);
            _dataStoreMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SyncPodcasts_WhenPodcastDetailPageTypeIdIsEmpty_ThrowException()
        {
            // arrange
            string rssFeed = "http://example.com/rss";
            var tenantId = Guid.NewGuid();
            var command = new EnableSpotifyPodcastSyncCommand(rssFeed);
            var cancellationToken = CancellationToken.None;

            ConfigureTestXml();

            _podcastRepositoryMock.Setup(s => s.AnyWithFiltersDisabled(It.IsAny<Expression<Func<Podcast, bool>>>())).Returns(false);
            _podcastRepositoryMock.Setup(s => s.FirstOrDefaultAsync(It.IsAny<PodcastByAudioUrlSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Podcast());
            _pageTypeRepositoryMock.Setup(s => s.FirstOrDefaultAsync(It.IsAny<PageTypeFromSystemKeySpecification>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Guid.Empty);

            // Act
            var act = async () => await _handler.SyncPodcasts(command, tenantId, rssFeed, CancellationToken.None);

            // Assert

            var test = act;

            await Assert.ThrowsAsync<PodcastSyncException>(act);

            await act.Should().ThrowAsync<PodcastSyncException>("podcastDetailPageTypeId is Empty");
        }

        private void ConfigureTestXml()
        {
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(async (HttpRequestMessage request, CancellationToken token) =>
                {
                    var response = new HttpResponseMessage();
                    string xmlFilePath = "Samples/TestSpotifyData.xml";

                    response.Content = new StringContent(await File.ReadAllTextAsync(xmlFilePath, token));

                    return response;
                })
                .Verifiable();
        }

        public class TestHttpClientFactory : IHttpClientFactory
        {
            private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

            public TestHttpClientFactory(Mock<HttpMessageHandler> mockHttpMessageHandler)
            {
                _mockHttpMessageHandler = mockHttpMessageHandler;
            }

            public HttpClient CreateClient(string name)
            {
                var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
                return httpClient;
            }
        }
    }
}
