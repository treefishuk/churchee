using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.X.Features.Tweets.Commands;
using Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync;
using Churchee.Module.X.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Linq.Expressions;
using System.Net;

namespace Churchee.Module.X.Tests.Features.Tweets.Commands.EnableTweetsSync
{
    public class EnableTweetsSyncCommandHandlerTests
    {
        private readonly Mock<IJobService> _jobService = new();
        private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
        private readonly Mock<IDataStore> _dataStore = new();
        private readonly Mock<ICurrentUser> _currentUser = new();
        private readonly Mock<ISettingStore> _setting_store = new();
        private readonly Mock<ISettingStore> _settingStore = new();
        private readonly Mock<ILogger<EnableTweetsSyncCommandHandler>> _logger = new();
        private readonly Mock<IRepository<Token>> _tokenRepo = new();
        private readonly Mock<IRepository<ViewTemplate>> _view_template_repo = new();
        private readonly Mock<IRepository<ViewTemplate>> _viewTemplateRepo = new();
        private readonly Mock<IRepository<MediaFolder>> _mediaFolderRepo = new();

        private readonly Guid _tenantId = Guid.NewGuid();

        public EnableTweetsSyncCommandHandlerTests()
        {
            _dataStore.Setup(x => x.GetRepository<Token>()).Returns(_tokenRepo.Object);
            _dataStore.Setup(x => x.GetRepository<ViewTemplate>()).Returns(_viewTemplateRepo.Object);
            _dataStore.Setup(x => x.GetRepository<MediaFolder>()).Returns(_mediaFolderRepo.Object);
            _currentUser.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(_tenantId);
        }

        [Fact]
        public async Task Handle_ShouldCreateTokenAndSettings_AndScheduleJob_WhenAllIsValid()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.OK, "{\"data\":{\"id\":\"12345\"}}");
            var command = new EnableTweetsSyncCommand("account", "token");

            _viewTemplateRepo.Setup(x => x.AnyWithFiltersDisabled(It.IsAny<System.Linq.Expressions.Expression<Func<ViewTemplate, bool>>>())).Returns(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _tokenRepo.Verify(x => x.Create(It.IsAny<Token>()), Times.Once);
            _settingStore.Verify(x => x.AddOrUpdateSetting(It.IsAny<Guid>(), _tenantId, "X/Twitter UserName", "account"), Times.Once);
            _jobService.Verify(x => x.ScheduleJob<SyncTweets>(It.IsAny<string>(), It.IsAny<Expression<Func<SyncTweets, Task>>>(), It.IsAny<Func<string>>()), Times.Once);
            _jobService.Verify(x => x.QueueJob<SyncTweets>(It.IsAny<Expression<Func<SyncTweets, Task>>>()), Times.Once);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenStoreUserIdFails()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.BadRequest, "");
            var command = new EnableTweetsSyncCommand("account", "token");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Description.Contains("Failed to get user ID from Twitter API"));
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenJobSchedulingThrows()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.OK, "{\"data\":{\"id\":\"12345\"}}");
            var command = new EnableTweetsSyncCommand("account", "token");

            _viewTemplateRepo.Setup(x => x.AnyWithFiltersDisabled(It.IsAny<System.Linq.Expressions.Expression<Func<ViewTemplate, bool>>>())).Returns(false);
            _jobService.Setup(x => x.ScheduleJob<SyncTweets>(It.IsAny<string>(), It.IsAny<Expression<Func<SyncTweets, Task>>>(), It.IsAny<Func<string>>()))
                .Throws(new Exception("Schedule error"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Description.Contains("Failed to schedule job for syncing tweets"));
        }

        [Fact]
        public async Task Handle_CreatesViewTemplate_IfNotExists()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.OK, "{\"data\":{\"id\":\"12345\"}}");
            var command = new EnableTweetsSyncCommand("testuser", "token");

            _viewTemplateRepo.Setup(x => x.AnyWithFiltersDisabled(It.IsAny<System.Linq.Expressions.Expression<Func<ViewTemplate, bool>>>())).Returns(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _viewTemplateRepo.Verify(x => x.Create(It.IsAny<ViewTemplate>()), Times.Once);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeast(2));
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_DoesNotCreateViewTemplate_IfExists()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.OK, "{\"data\":{\"id\":\"12345\"}}");
            var command = new EnableTweetsSyncCommand("testuser", "token");

            _viewTemplateRepo.Setup(x => x.AnyWithFiltersDisabled(It.IsAny<System.Linq.Expressions.Expression<Func<ViewTemplate, bool>>>())).Returns(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _viewTemplateRepo.Verify(x => x.Create(It.IsAny<ViewTemplate>()), Times.Never);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_StripsAtSymbol_FromAccountName_BeforeSavingSetting()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.OK, "{\"data\":{\"id\":\"12345\"}}");
            var command = new EnableTweetsSyncCommand("@account", "token");

            _viewTemplateRepo.Setup(x => x.AnyWithFiltersDisabled(It.IsAny<System.Linq.Expressions.Expression<Func<ViewTemplate, bool>>>())).Returns(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _settingStore.Verify(
                x => x.AddOrUpdateSetting(
                    It.IsAny<Guid>(),
                    _tenantId,
                    "X/Twitter UserName",
                    "account" // Should be without '@'
                ),
                Times.Once
            );
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenQueueJobThrows()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.OK, "{\"data\":{\"id\":\"12345\"}}");
            var command = new EnableTweetsSyncCommand("testuser", "token");

            _viewTemplateRepo.Setup(x => x.AnyWithFiltersDisabled(It.IsAny<System.Linq.Expressions.Expression<Func<ViewTemplate, bool>>>())).Returns(false);
            _jobService.Setup(x => x.QueueJob<SyncTweets>(It.IsAny<Expression<Func<SyncTweets, Task>>>())).Throws(new Exception("Queue error"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Description.Contains("Failed to queue job for syncing tweets"));
        }

        [Fact]
        public async Task StoreUserId_ReturnsError_WhenResponseIsEmpty()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.OK, "");
            var method = handler.GetType().GetMethod("StoreUserId");

            // Act
            var response = await handler.StoreUserId("testuser", "token", _tenantId, CancellationToken.None);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Contains(response.Errors, e => e.Description.Contains("Failed to get user ID from Twitter API"));
        }

        [Fact]
        public async Task StoreUserId_ReturnsError_WhenDeserializationFails()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.OK, "{\"unexpected\":\"value\"}");

            // Act
            var response = await handler.StoreUserId("testuser", "token", _tenantId, CancellationToken.None);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Contains(response.Errors, e => e.Description.Contains("Failed to deserialize user ID from Twitter API response"));
        }

        [Fact]
        public async Task StoreUserId_ReturnsError_WhenHttpFails()
        {
            // Arrange
            var handler = CreateHandlerWithHttpClientMock(HttpStatusCode.BadRequest, "");

            // Act
            var response = await handler.StoreUserId("testuser", "token", _tenantId, CancellationToken.None);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Contains(response.Errors, e => e.Description.Contains("Failed to get user ID from Twitter API"));
        }

        private EnableTweetsSyncCommandHandler CreateHandlerWithHttpClientMock(HttpStatusCode statusCode, string content)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                });

            var httpClient = new HttpClient(httpMessageHandler.Object);
            _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            return new EnableTweetsSyncCommandHandler(
                _jobService.Object,
                _httpClientFactory.Object,
                _dataStore.Object,
                _currentUser.Object,
                _settingStore.Object,
                _logger.Object
            );
        }
    }

}