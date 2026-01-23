using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Churchee.Module.X.Exceptions;
using Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync;
using Churchee.Module.X.Jobs;
using Moq;
using Moq.Protected;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;

namespace Churchee.Module.X.Tests.Jobs
{
    public class SyncTweetsTests
    {
        private readonly Guid _tenantId = Guid.NewGuid();

        private readonly Mock<IDataStore> _dataStore;
        private readonly Mock<IRepository<Token>> _tokenRepo;
        private readonly Mock<IRepository<MediaItem>> _mediaItemsRepo;
        private readonly Mock<IRepository<MediaFolder>> _mediaFolderRepo;
        private readonly Mock<ISettingStore> _settingStore;

        public SyncTweetsTests()
        {
            _dataStore = new Mock<IDataStore>();
            _tokenRepo = new Mock<IRepository<Token>>();
            _mediaItemsRepo = new Mock<IRepository<MediaItem>>();
            _mediaFolderRepo = new Mock<IRepository<MediaFolder>>();
            _settingStore = new Mock<ISettingStore>();

            // common baseline: bearer token retrieval and default setting value
            _tokenRepo
                .Setup(t => t.FirstOrDefaultAsync(It.IsAny<GetTokenByKeySpecification>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("bearer");

            _dataStore.Setup(x => x.GetRepository<Token>()).Returns(_tokenRepo.Object);
            _dataStore.Setup(x => x.GetRepository<MediaItem>()).Returns(_mediaItemsRepo.Object);
            _dataStore.Setup(x => x.GetRepository<MediaFolder>()).Returns(_mediaFolderRepo.Object);

            _settingStore.Setup(s => s.GetSettingValue(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync("123");

            // default SaveChanges - tests will override where needed
            _dataStore.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        }

        private static HttpClient CreateHttpClient(HttpStatusCode statusCode, string content)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content ?? string.Empty)
                })
                .Verifiable();

            return new HttpClient(handlerMock.Object);
        }

        private SyncTweets CreateSut(HttpClient httpClient)
        {
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            return new SyncTweets(httpClientFactory.Object, _dataStore.Object, _settingStore.Object);
        }

        [Fact]
        public async Task ExecuteAsync_Returns_When_TooManyRequests()
        {
            // Arrange
            var httpClient = CreateHttpClient(HttpStatusCode.TooManyRequests, string.Empty);

            var sut = CreateSut(httpClient);

            // Act
            await sut.ExecuteAsync(_tenantId, CancellationToken.None);

            // Assert
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Throws_When_NonSuccess()
        {
            // Arrange
            var httpClient = CreateHttpClient(HttpStatusCode.InternalServerError, "oops");

            var sut = CreateSut(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<XSyncException>(() => sut.ExecuteAsync(_tenantId, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_Throws_When_DeserializedResponseIsNull()
        {
            // Arrange - body = "null" will deserialize to null
            var httpClient = CreateHttpClient(HttpStatusCode.OK, "null");

            var sut = CreateSut(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<XSyncException>(() => sut.ExecuteAsync(_tenantId, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_Returns_When_NoTweets()
        {
            // Arrange - JSON with empty Tweets list
            var response = new GetTweetsApiResponse();

            string json = JsonSerializer.Serialize(response);

            var httpClient = CreateHttpClient(HttpStatusCode.OK, json);

            var sut = CreateSut(httpClient);

            // Act
            await sut.ExecuteAsync(_tenantId, CancellationToken.None);

            // Assert - no media repos touched
            _mediaItemsRepo.Verify(m => m.Create(It.IsAny<MediaItem>()), Times.Never);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Skips_When_MediaItem_AlreadyExists()
        {
            // Arrange - one tweet present but media item exists
            var response = new GetTweetsApiResponse()
            {
                Tweets = [new Tweet { Text = "hi", Id = "t1", CreatedAt = DateTime.Parse("2024-01-01T12:00:00Z") }]
            };

            string json = JsonSerializer.Serialize(response);

            var httpClient = CreateHttpClient(HttpStatusCode.OK, json);

            _mediaItemsRepo.Setup(m => m.AnyWithFiltersDisabled(It.IsAny<Expression<Func<MediaItem, bool>>>())).Returns(true);

            _mediaFolderRepo.Setup(m => m.FirstOrDefaultAsync(It.IsAny<MediaFolderByNameSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MediaFolder(_tenantId, "Tweets", ""));

            var sut = CreateSut(httpClient);

            // Act
            await sut.ExecuteAsync(_tenantId, CancellationToken.None);

            // Assert - create not called because exists
            _mediaItemsRepo.Verify(m => m.Create(It.IsAny<MediaItem>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Creates_New_Tweets_And_Saves()
        {
            // Arrange - one new tweet
            var response = new GetTweetsApiResponse()
            {
                Tweets = [new Tweet { Text = "hi", Id = "t1", CreatedAt = DateTime.Parse("2024-01-01T12:00:00Z") }]
            };

            string json = JsonSerializer.Serialize(response);

            var httpClient = CreateHttpClient(HttpStatusCode.OK, json);

            _mediaItemsRepo.Setup(m => m.AnyWithFiltersDisabled(It.IsAny<Expression<Func<MediaItem, bool>>>())).Returns(false);
            _mediaItemsRepo.Setup(m => m.Create(It.IsAny<MediaItem>()));

            var tweetsFolder = new MediaFolder(_tenantId, "Tweets", "");
            _mediaFolderRepo.Setup(m => m.FirstOrDefaultAsync(It.IsAny<MediaFolderByNameSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tweetsFolder);

            _dataStore.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var sut = CreateSut(httpClient);

            // Act
            await sut.ExecuteAsync(_tenantId, CancellationToken.None);

            // Assert
            _mediaItemsRepo.Verify(m => m.Create(It.Is<MediaItem>(mi => mi.Title.Contains("Tweet: t1"))), Times.Once);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Includes_SinceId_When_LastTweetExists()
        {
            // Arrange - latest tweet title exists in the repo so we expect since_id to be appended
            string lastTitle = "Tweet: t42";

            HttpRequestMessage capturedRequest = null;

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new GetTweetsApiResponse()))
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            // Ensure GetTweetsUrl will pick up lastTitle
            _mediaItemsRepo.Setup(m => m.FirstOrDefaultAsync(It.IsAny<LatestTweetMediaItemSpecification>(), It.IsAny<Expression<Func<MediaItem, string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lastTitle);

            var tweetsFolder = new MediaFolder(_tenantId, "Tweets", "");
            _mediaFolderRepo.Setup(m => m.FirstOrDefaultAsync(It.IsAny<MediaFolderByNameSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tweetsFolder);

            var sut = CreateSut(httpClient);

            // Act
            await sut.ExecuteAsync(_tenantId, CancellationToken.None);

            // Assert - we captured the outgoing request and it should include the since_id query param
            Assert.NotNull(capturedRequest);
            Assert.Contains("since_id=t42", capturedRequest!.RequestUri!.ToString());
        }
    }
}