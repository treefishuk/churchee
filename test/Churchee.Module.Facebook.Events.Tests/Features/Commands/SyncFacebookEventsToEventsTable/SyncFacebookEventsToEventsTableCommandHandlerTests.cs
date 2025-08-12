using Ardalis.Specification;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Facebook.Events.Features.Commands;
using Churchee.Module.Facebook.Events.Features.Commands.SyncFacebookEventsToEventsTable;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tokens.Entities;
using Churchee.Test.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;

namespace Churchee.Module.Facebook.Events.Tests.Features.Commands.SyncFacebookEventsToEventsTable
{
    public class SyncFacebookEventsToEventsTableCommandHandlerTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
        private readonly Mock<ISettingStore> _settingStore = new();
        private readonly Mock<IDataStore> _dataStore = new();
        private readonly Mock<IBlobStore> _blobStore = new();
        private readonly Mock<ICurrentUser> _currentUser = new();
        private readonly Mock<IJobService> _jobService = new();
        private readonly Mock<ILogger<SyncFacebookEventsToEventsTableCommandHandler>> _logger = new();

        private SyncFacebookEventsToEventsTableCommandHandler CreateHandler()
        {
            return new SyncFacebookEventsToEventsTableCommandHandler(
                _httpClientFactory.Object,
                _settingStore.Object,
                _dataStore.Object,
                _blobStore.Object,
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
                It.IsAny<Expression<Func<Task>>>(),
                It.IsAny<Func<string>>()), Times.Once);

            _jobService.Verify(x => x.QueueJob(It.IsAny<Expression<Func<Task>>>()), Times.Once);

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

        [Fact]
        public async Task SyncFacebookEvents_NoFeedItems_DoesNothing()
        {
            // Arrange
            var handler = CreateHandler();
            var tenantId = Guid.NewGuid();

            var tokenRepoMock = new Mock<IRepository<Token>>();
            var eventRepoMock = new Mock<IRepository<Event>>();
            var pageTypeRepoMock = new Mock<IRepository<PageType>>();

            _dataStore.Setup(x => x.GetRepository<Token>()).Returns(tokenRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Event>()).Returns(eventRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<PageType>()).Returns(pageTypeRepoMock.Object);

            tokenRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Token>>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("access-token");
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("page-id");

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, "{\"data\":[]}"));

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            // Act
            await handler.SyncFacebookEvents(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(x => x.Create(It.IsAny<Event>()), Times.Never);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SyncFacebookEvents_FeedItems_NoEventStories_DoesNothing()
        {
            // Arrange
            var handler = CreateHandler();
            var tenantId = Guid.NewGuid();

            var tokenRepoMock = new Mock<IRepository<Token>>();
            var eventRepoMock = new Mock<IRepository<Event>>();
            var pageTypeRepoMock = new Mock<IRepository<PageType>>();

            _dataStore.Setup(x => x.GetRepository<Token>()).Returns(tokenRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Event>()).Returns(eventRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<PageType>()).Returns(pageTypeRepoMock.Object);

            tokenRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Token>>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("access-token");
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("page-id");

            var feed = new
            {
                data = new[]
                {
                    new { id = "page-id_1", story = "Some other story" }
                }
            };
            string feedJson = JsonSerializer.Serialize(feed);

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, feedJson));

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            // Act
            await handler.SyncFacebookEvents(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(x => x.Create(It.IsAny<Event>()), Times.Never);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SyncFacebookEvents_FeedItems_EventStories_AlreadyExists_DoesNotCreate()
        {
            // Arrange
            var handler = CreateHandler();
            var tenantId = Guid.NewGuid();

            var tokenRepoMock = new Mock<IRepository<Token>>();
            var eventRepoMock = new Mock<IRepository<Event>>();
            var pageTypeRepoMock = new Mock<IRepository<PageType>>();

            _dataStore.Setup(x => x.GetRepository<Token>()).Returns(tokenRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Event>()).Returns(eventRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<PageType>()).Returns(pageTypeRepoMock.Object);

            tokenRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Token>>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("access-token");
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("page-id");

            var feed = new
            {
                data = new[]
                {
                    new { id = "page-id_1", story = "Someone created an event" }
                }
            };
            string feedJson = JsonSerializer.Serialize(feed);

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, feedJson));

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            // Simulate event already exists
            eventRepoMock.Setup(x => x.ApplySpecification(It.IsAny<ISpecification<Event>>(), false)).Returns(new[] { new Event() }.AsQueryable());

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PageType>>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            await handler.SyncFacebookEvents(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(x => x.Create(It.IsAny<Event>()), Times.Never);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SyncFacebookEvents_FeedItems_EventStories_NewEvent_CreatesEvent()
        {
            // Arrange
            var handler = CreateHandler();
            var tenantId = Guid.NewGuid();

            var tokenRepoMock = new Mock<IRepository<Token>>();
            var eventRepoMock = new Mock<IRepository<Event>>();
            var pageTypeRepoMock = new Mock<IRepository<PageType>>();
            var pageRepoMock = new Mock<IRepository<Page>>();

            _dataStore.Setup(x => x.GetRepository<Token>()).Returns(tokenRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Event>()).Returns(eventRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<PageType>()).Returns(pageTypeRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Page>()).Returns(pageRepoMock.Object);

            tokenRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Token>>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("access-token");
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("page-id");

            var feed = new
            {
                data = new[]
                {
                    new { id = "page-id_1", story = "Someone created an event" }
                }
            };
            string feedJson = JsonSerializer.Serialize(feed);

            // Simulate Facebook event details
            string fbEventJson = JsonSerializer.Serialize(new
            {
                id = "1",
                name = "Test Event",
                description = "Test Description",
                place = (object?)null,
                start_time = DateTime.UtcNow,
                end_time = DateTime.UtcNow.AddHours(1),
                cover = (object?)null
            });

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, feedJson, fbEventJson))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            var httpClient2 = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK));
            httpClient2.BaseAddress = new Uri("http://localhost/");

            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient2);

            // Simulate event does not exist
            eventRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Event>>(), It.IsAny<Expression<Func<Event, Event>>>(), It.IsAny<CancellationToken>()));

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PageType>>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            await handler.SyncFacebookEvents(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(x => x.Create(It.IsAny<Event>()), Times.Once);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SyncFacebookEvents_Exception_LogsError()
        {
            // Arrange
            var handler = CreateHandler();
            var tenantId = Guid.NewGuid();

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Throws(new Exception("fail"));

            // Act
            await handler.SyncFacebookEvents(tenantId, CancellationToken.None);

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString() == "Error Syncing Facebook Events" && t.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

        }

    }
}
