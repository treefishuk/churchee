using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Churchee.Module.Videos.Entities;
using Churchee.Module.YouTube.Exceptions;
using Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync;
using Churchee.Module.YouTube.Helpers;
using Churchee.Module.YouTube.Jobs;
using Churchee.Test.Helpers;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;

namespace Churchee.Module.YouTube.Tests.Jobs
{
    public class FullSyncYouTubeVideosJobTests
    {
        private readonly Mock<IRepository<Video>> _mockVideoRepo;

        public FullSyncYouTubeVideosJobTests()
        {
            _mockVideoRepo = new Mock<IRepository<Video>>();
        }

        [Fact]
        public async Task YouTubeExceptionThrown_When_HttpClientResponse_Not_Success()
        {
            // Arrange
            var mockSettingStore = new Mock<ISettingStore>();

            var mockDataStore = SetupMockDataStore();

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.Forbidden));

            mockHttpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new FullSyncYouTubeVideosJob(mockSettingStore.Object, mockDataStore.Object, mockHttpClientFactory.Object);

            var appTenantId = Guid.NewGuid();

            // Act
            var act = () => cut.ExecuteAsync(appTenantId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<YouTubeSyncException>();
        }

        [Fact]
        public async Task SaveChangesAsync_NotCalled_If_ResponseEmpty()
        {
            // Arrange
            var mockSettingStore = new Mock<ISettingStore>();

            var mockDataStore = SetupMockDataStore();

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var responseContent = new GetYouTubeVideosApiResponse();

            var serializedContent = JsonSerializer.Serialize(responseContent);

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, serializedContent));

            mockHttpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new FullSyncYouTubeVideosJob(mockSettingStore.Object, mockDataStore.Object, mockHttpClientFactory.Object);

            var appTenantId = Guid.NewGuid();

            // Act
            await cut.ExecuteAsync(appTenantId, CancellationToken.None);

            // Assert
            mockDataStore.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Two_New_Entries_Added_If_Response_Contains_Two_New_Items()
        {
            // Arrange
            var mockSettingStore = new Mock<ISettingStore>();

            string channelId = "123456";

            mockSettingStore.Setup(s => s.GetSettingValue(SettingKeys.ChannelId, It.IsAny<Guid>())).ReturnsAsync(channelId);
            mockSettingStore.Setup(s => s.GetSettingValue(SettingKeys.VideosPageName, It.IsAny<Guid>())).ReturnsAsync("Watch");

            var mockDataStore = SetupMockDataStore();

            _mockVideoRepo.Setup(s => s.AnyWithFiltersDisabled(It.IsAny<Expression<Func<Video, bool>>>())).Returns(false);

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var responseContent = GetTestResponseContent(channelId);

            var serializedContent = JsonSerializer.Serialize(responseContent);

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, serializedContent));

            mockHttpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new FullSyncYouTubeVideosJob(mockSettingStore.Object, mockDataStore.Object, mockHttpClientFactory.Object);

            var appTenantId = Guid.NewGuid();

            // Act
            await cut.ExecuteAsync(appTenantId, CancellationToken.None);

            // Assert
            mockDataStore.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task New_New_Entries_Added_If_Response_Contains_Two_Existing_Items()
        {
            // Arrange
            var mockSettingStore = new Mock<ISettingStore>();

            string channelId = "123456";

            mockSettingStore.Setup(s => s.GetSettingValue(SettingKeys.ChannelId, It.IsAny<Guid>())).ReturnsAsync(channelId);
            mockSettingStore.Setup(s => s.GetSettingValue(SettingKeys.VideosPageName, It.IsAny<Guid>())).ReturnsAsync("Watch");

            var mockDataStore = SetupMockDataStore();

            _mockVideoRepo.Setup(s => s.AnyWithFiltersDisabled(It.IsAny<Expression<Func<Video, bool>>>())).Returns(true);

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var responseContent = GetTestResponseContent(channelId);

            var serializedContent = JsonSerializer.Serialize(responseContent);

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, serializedContent));

            mockHttpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new FullSyncYouTubeVideosJob(mockSettingStore.Object, mockDataStore.Object, mockHttpClientFactory.Object);

            var appTenantId = Guid.NewGuid();

            // Act
            await cut.ExecuteAsync(appTenantId, CancellationToken.None);

            // Assert
            mockDataStore.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        private static GetYouTubeVideosApiResponse GetTestResponseContent(string channelId)
        {
            return new GetYouTubeVideosApiResponse()
            {
                Items = new List<YouTubeVideo>
                {
                    new() {
                        Id = new Id
                        {
                            VideoId = "1"
                        },
                        Snippet = new Snippet
                        {
                            ChannelId = channelId,
                            Title = "Video Title",
                            Description = "Description",
                            PublishTime = DateTime.Now.AddDays(-1)

                        }
                    },
                    new() {
                        Id = new Id
                        {
                            VideoId = "2"
                        },
                        Snippet = new Snippet
                        {
                            ChannelId = channelId,
                            Title = "Video Title",
                            Description = "Description",
                            PublishTime = DateTime.Now.AddDays(-1)
                        }
                    }
                }
            };
        }

        private Mock<IDataStore> SetupMockDataStore()
        {

            var mockDataStore = new Mock<IDataStore>();
            var mockTokenRepository = new Mock<IRepository<Token>>();
            mockTokenRepository.Setup(s => s.FirstOrDefaultAsync(It.IsAny<GetTokenByKeySpecification>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>())).ReturnsAsync("key");

            var mockPageTypeRepository = new Mock<IRepository<PageType>>();
            mockPageTypeRepository.Setup(s => s.FirstOrDefaultAsync(It.IsAny<PageTypeFromSystemKeySpecification>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid());

            mockDataStore.Setup(s => s.GetRepository<Token>()).Returns(mockTokenRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<Video>()).Returns(_mockVideoRepo.Object);
            mockDataStore.Setup(s => s.GetRepository<PageType>()).Returns(mockPageTypeRepository.Object);
            return mockDataStore;
        }
    }
}
