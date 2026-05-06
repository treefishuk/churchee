using Ardalis.Specification;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Facebook.Events.API;
using Churchee.Module.Facebook.Events.Jobs;
using Churchee.Module.Facebook.Events.Specifications;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tokens.Entities;
using Churchee.Test.Helpers;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;

namespace Churchee.Module.Facebook.Events.Tests.Jobs
{
    public class SyncFacebookEventsJobTests
    {

        private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
        private readonly Mock<ISettingStore> _settingStore = new();
        private readonly Mock<IDataStore> _dataStore = new();
        private readonly Mock<IBlobStore> _blobStore = new();
        private readonly Mock<IStores> _storesMock = new();
        private readonly Mock<ILogger<SyncFacebookEventsJob>> _logger = new();
        private readonly Mock<IJobService> _jobService = new();
        private readonly Mock<IImageProcessor> _imageProcessor = new();
        private readonly Mock<IRepository<Token>> tokenRepoMock = new();
        private readonly Mock<IRepository<Event>> eventRepoMock = new();
        private readonly Mock<IRepository<EventDate>> eventDateRepoMock = new();
        private readonly Mock<IRepository<PageType>> pageTypeRepoMock = new();
        private readonly Mock<IRepository<Page>> pageRepoMock = new();
        private readonly Guid tenantId;

        public SyncFacebookEventsJobTests()
        {
            tenantId = Guid.NewGuid();

            _dataStore.Setup(x => x.GetRepository<Token>()).Returns(tokenRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Event>()).Returns(eventRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<PageType>()).Returns(pageTypeRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Page>()).Returns(pageRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<EventDate>()).Returns(eventDateRepoMock.Object);

            tokenRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Token>>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync("access-token");

            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("page-id");

            _logger.Setup(s => s.IsEnabled(LogLevel.Error)).Returns(true);

            _storesMock.Setup(s => s.SettingStore).Returns(_settingStore.Object);
            _storesMock.Setup(s => s.DataStore).Returns(_dataStore.Object);
            _storesMock.Setup(s => s.BlobStore).Returns(_blobStore.Object);
        }

        [Fact]
        public async Task SyncFacebookEvents_NoFeedItems_DoesNothing()
        {
            // Arrange
            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, "{\"data\":[]}"));

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(x => x.Create(It.IsAny<Event>()), Times.Never);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SyncFacebookEvents_EmptyResponse_DoesNothing()
        {
            // Arrange
            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, string.Empty));

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(x => x.Create(It.IsAny<Event>()), Times.Never);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SyncFacebookEvents_FeedItems_NoEventStories_DoesNothing()
        {
            // Arrange
            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

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
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(x => x.Create(It.IsAny<Event>()), Times.Never);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SyncFacebookEvents_FeedItems_EventStories_NewEvent_CreatesEvent()
        {
            // Arrange
            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), It.IsAny<Guid>())).ReturnsAsync((string?)null);

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

            var httpClient2 = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient2);

            // Simulate event does not exist
            eventRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Event>>(), It.IsAny<Expression<Func<Event, Event>>>(), It.IsAny<CancellationToken>()));

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PageType>>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(x => x.Create(It.IsAny<Event>()), Times.Once);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SyncFacebookEvents_Exception_LogsError()
        {
            // Arrange
            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);
            var tenantId = Guid.NewGuid();

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Throws(new Exception("fail"));

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

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

        [Fact]
        public async Task SyncFacebookEvents_FeedItems_EventStories_ExistingEvent_UpdatesEvent_NoImageChange()
        {
            // Arrange
            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), tenantId)).ReturnsAsync((string?)null);

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            var feed = new
            {
                data = new[]
                {
                    new { id = "page-id_1", story = "Someone created an event" }
                }
            };
            string feedJson = JsonSerializer.Serialize(feed);

            // Simulate Facebook event details
            string fbEventJson = JsonSerializer.Serialize(new FacebookEventResult
            {
                Id = "1",
                Name = "Test Event",
                Description = "Test Description",
                Place = new Place { Location = new Location { Street = "New Street" } },
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Cover = new Cover { Source = "http://localhost/123456" }
            });

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, feedJson, fbEventJson))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            // Simulate image has not changed

            string imageContent = "SAMEIMAGE";

            var httpClient2 = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, imageContent))
            {
                BaseAddress = new Uri("http://localhost/")
            };


            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient2);

            var existingEvent = new Event.Builder()
                .SetSourceId("1")
                .SetTitle("Old Title")
                .SetDescription("Old Description")
                .SetStreet("Old Street")
                .Build();

            existingEvent.SetImageCheckHash("E2E87C7BCB3145C0177A94CC9BE7769C3CF3F2161F09C65ADF1CE78B1892D5DB");

            // Simulate event does not exist
            eventRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetEventByFacebookIdSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingEvent);

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PageType>>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            existingEvent.Title.Should().Be("Test Event");
            existingEvent.Description.Should().Be("Test Description");
            existingEvent.Street.Should().Be("New Street");

            _imageProcessor.Verify(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SyncFacebookEvents_FeedItems_EventStories_ExistingEvent_UpdatesEvent_ImageChange()
        {
            // Arrange
            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), tenantId)).ReturnsAsync((string?)null);

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            var feed = new
            {
                data = new[]
                {
                    new { id = "page-id_1", story = "Someone created an event" }
                }
            };

            string feedJson = JsonSerializer.Serialize(feed);

            // Simulate Facebook event details
            string fbEventJson = JsonSerializer.Serialize(new FacebookEventResult
            {
                Id = "1",
                Name = "Test Event",
                Description = "Test Description",
                Place = new Place { Location = new Location { Street = "New Street" } },
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Cover = new Cover { Source = "http://localhost/123456" }
            });

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, feedJson, fbEventJson))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            // Simulate image has not changed

            string imageContent = "NOTTHESAMEIMAGE";

            var httpClient2 = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, imageContent))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient2);

            var existingEvent = new Event.Builder()
                .SetSourceId("1")
                .SetTitle("Old Title")
                .SetDescription("Old Description")
                .SetStreet("Old Street")
                .Build();

            existingEvent.SetImageCheckHash("E2E87C7BCB3145C0177A94CC9BE7769C3CF3F2161F09C65ADF1CE78B1892D5DB");

            // Simulate event does not exist
            eventRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetEventByFacebookIdSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingEvent);

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PageType>>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            existingEvent.Title.Should().Be("Test Event");
            existingEvent.Description.Should().Be("Test Description");
            existingEvent.Street.Should().Be("New Street");

            _imageProcessor.Verify(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task SyncFacebookEvents_ImageProcessor_Logs_Exception_When_Thrown()
        {
            // Arrange
            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), tenantId)).ReturnsAsync((string?)null);

            _imageProcessor.Setup(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            var feed = new
            {
                data = new[]
                {
                    new { id = "page-id_1", story = "Someone created an event" }
                }
            };

            string feedJson = JsonSerializer.Serialize(feed);

            // Simulate Facebook event details
            string fbEventJson = JsonSerializer.Serialize(new FacebookEventResult
            {
                Id = "1",
                Name = "Test Event",
                Description = "Test Description",
                Place = new Place { Location = new Location { Street = "New Street" } },
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Cover = new Cover { Source = "http://localhost/123456" }
            });

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, feedJson, fbEventJson))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            // Simulate image has not changed

            string imageContent = "NOTTHESAMEIMAGE";

            var httpClient2 = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, imageContent))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient2);

            var existingEvent = new Event.Builder()
                .SetSourceId("1")
                .SetTitle("Old Title")
                .SetDescription("Old Description")
                .SetStreet("Old Street")
                .Build();

            existingEvent.SetImageCheckHash("E2E87C7BCB3145C0177A94CC9BE7769C3CF3F2161F09C65ADF1CE78B1892D5DB");

            // Simulate event does not exist
            eventRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetEventByFacebookIdSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingEvent);

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PageType>>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            existingEvent.Title.Should().Be("Test Event");
            existingEvent.Description.Should().Be("Test Description");
            existingEvent.Street.Should().Be("New Street");

            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        [Fact]
        public async Task ConvertImageToLocalImage_ImageProcessorThrows_LogsError()
        {
            // Arrange - create a feed + event with cover where image fetch is OK but processor throws
            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            var feed = new
            {
                data = new[]
                {
                    new { id = "page-id_1", story = "Someone created an event" }
                }
            };
            string feedJson = JsonSerializer.Serialize(feed);

            string fbEventJson = JsonSerializer.Serialize(new FacebookEventResult
            {
                Id = "1",
                Name = "Test Event",
                Description = "Test Description",
                Place = new Place { Location = new Location { Street = "New Street" } },
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Cover = new Cover { Source = "http://localhost/image" }
            });

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, feedJson, fbEventJson))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(httpClient);

            // Image http client returns OK with some content
            var httpClientImage = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, "IMAGEBYTES"))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClientImage);

            // Make the image processor throw when converting
            _imageProcessor.Setup(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("convert failed"));

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PageType>>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert - error logged from ConvertImageToLocalImage catch
            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateEventDateTime_UKSetAsTimezone_UsesUKTimeFormat()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            var timeProviderMock = new Mock<TimeProvider>();

            timeProviderMock
                .Setup(t => t.GetUtcNow())
                .Returns(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero));

            timeProviderMock
                .Setup(t => t.LocalTimeZone)
                .Returns(TimeZoneInfo.Utc);

            var eventData = new FacebookEventResult
            {
                Id = "1",
                StartTime = new DateTime(2026, 03, 31, 14, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 03, 31, 16, 0, 0, DateTimeKind.Utc),
            };

            var existingDate = new EventDate();

            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), tenantId)).ReturnsAsync("Europe/London");

            eventDateRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<EventDatesForEventSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingDate);

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object, timeProviderMock.Object);

            // Act
            await cut.UpdateEventDateTime(eventData, eventId, tenantId, CancellationToken.None);

            // Assert
            var ukDateStart = new DateTime(2026, 03, 31, 15, 0, 0);
            var ukDateEnd = new DateTime(2026, 03, 31, 17, 0, 0);

            existingDate.Start.Should().Be(ukDateStart);
            existingDate.End.Should().Be(ukDateEnd);
        }

        [Fact]
        public async Task UpdateEventDateTime_NoTimeZoneSet_UsesUTC()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var timeProviderMock = new Mock<TimeProvider>();

            timeProviderMock
                .Setup(t => t.GetUtcNow())
                .Returns(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero));

            timeProviderMock
                .Setup(t => t.LocalTimeZone)
                .Returns(TimeZoneInfo.Utc);

            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), It.IsAny<Guid>())).ReturnsAsync((string?)null);

            var eventData = new FacebookEventResult
            {
                Id = "1",
                StartTime = new DateTime(2026, 03, 31, 15, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 03, 31, 17, 0, 0, DateTimeKind.Utc),
            };

            var existingDate = new EventDate();

            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), tenantId)).ReturnsAsync((string?)null);

            eventDateRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<EventDatesForEventSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingDate);

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object, timeProviderMock.Object);

            // Act
            await cut.UpdateEventDateTime(eventData, eventId, tenantId, CancellationToken.None);

            // Assert
            existingDate.Start.Should().Be(eventData.StartTime);
            existingDate.End.Should().Be(eventData.EndTime);
        }

        [Fact]
        public async Task UpdateEventDateTime_EmptyTimeZoneSet_UsesUTC()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            var eventData = new FacebookEventResult
            {
                Id = "1",
                StartTime = DateTime.Now.AddDays(3).ToUniversalTime(),
                EndTime = DateTime.Now.AddDays(3).AddHours(1).ToUniversalTime()
            };

            var existingDate = new EventDate();

            existingDate.Start = DateTime.Now.AddDays(1).ToUniversalTime();

            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), tenantId)).ReturnsAsync(string.Empty);

            eventDateRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<EventDatesForEventSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingDate);

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            // Act
            await cut.UpdateEventDateTime(eventData, eventId, tenantId, CancellationToken.None);

            // Assert
            existingDate.Start.Should().Be(eventData.StartTime);
            existingDate.End.Should().Be(eventData.EndTime);
        }

        [Fact]
        public async Task CreateNewEvent_WithParentPage_SetsParentSlugAndParentId()
        {
            // Arrange
            var createdEvent = (Event?)null;

            eventRepoMock.Setup(r => r.Create(It.IsAny<Event>()))
                         .Callback<Event>(e => createdEvent = e);

            var parentPage = new Page(Guid.NewGuid(), "Title", "/events/custom", "Description", Guid.NewGuid(), Guid.NewGuid(), false);

            pageRepoMock.Setup(p => p.FirstOrDefaultAsync(It.IsAny<EventListingPageSpecification>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(parentPage);

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PageType>>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), It.IsAny<Guid>())).ReturnsAsync((string?)null);

            var feed = new
            {
                data = new[]
                {
                    new { id = "page-id_1", story = "Someone created an event" }
                }
            };

            string feedJson = JsonSerializer.Serialize(feed);

            string fbEventJson = JsonSerializer.Serialize(new
            {
                id = "1",
                name = "New Event",
                description = "desc",
                place = (object?)null,
                start_time = DateTime.UtcNow.AddDays(1),
                end_time = DateTime.UtcNow.AddDays(1).AddHours(1),
                cover = (object?)null
            });

            var fbClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, feedJson, fbEventJson))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(fbClient);

            // Default image client should not be needed
            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK)) { BaseAddress = new Uri("http://localhost/") });

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            createdEvent.Should().NotBeNull();
            createdEvent!.ParentId.Should().Be(parentPage.Id);
            createdEvent.Url.Should().Contain(parentPage.Url);
            _dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEventDateTime_StartBeforeNow_ReturnsWithoutUpdating()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var timeProviderMock = new Mock<TimeProvider>();

            timeProviderMock
                .Setup(t => t.GetUtcNow())
                .Returns(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero));

            timeProviderMock
                .Setup(t => t.LocalTimeZone)
                .Returns(TimeZoneInfo.Utc);

            // start is 5 minutes ago
            var eventData = new FacebookEventResult
            {
                Id = "1",
                StartTime = now.AddMinutes(-5),
                EndTime = now.AddHours(1)
            };

            var existingDate = new EventDate { Start = DateTime.UtcNow.AddDays(5) };

            eventDateRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<EventDatesForEventSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingDate);

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object, timeProviderMock.Object);

            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), It.IsAny<Guid>())).ReturnsAsync(string.Empty);

            // Act
            await cut.UpdateEventDateTime(eventData, eventId, tenantId, CancellationToken.None);

            // Assert - start should not have been changed
            eventDateRepoMock.Verify(x => x.Create(It.IsAny<EventDate>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEventDateTime_DateDoesNotExist_CreatesNewEventDate()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            var eventData = new FacebookEventResult
            {
                Id = "1",
                StartTime = DateTime.UtcNow.AddDays(2),
                EndTime = DateTime.UtcNow.AddDays(2).AddHours(2)
            };

            EventDate? createdDate = null;

            eventDateRepoMock.Setup(x => x.Create(It.IsAny<EventDate>())).Callback<EventDate>(d => createdDate = d);

            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), It.IsAny<Guid>())).ReturnsAsync((string?)null);

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            // Act
            await cut.UpdateEventDateTime(eventData, eventId, tenantId, CancellationToken.None);

            // Assert
            eventDateRepoMock.Verify(x => x.Create(It.IsAny<EventDate>()), Times.Once);
            createdDate.Should().NotBeNull();
            createdDate!.EventId.Should().Be(eventId);
            createdDate.Start.Should().Be(eventData.StartTime);
            createdDate.End.Should().Be(eventData.EndTime);
        }

        [Fact]
        public async Task SyncFacebookEvents_CreateNewEvent_WithNoCover_DoesNotFetchImageClient()
        {
            // Arrange
            Event? createdEvent = null;

            eventRepoMock.Setup(r => r.Create(It.IsAny<Event>()))
                         .Callback<Event>(e => createdEvent = e);

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PageType>>(), It.IsAny<Expression<Func<PageType, Guid>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            _settingStore.Setup(x => x.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), It.IsAny<Guid>())).ReturnsAsync((string?)null);

            var feed = new
            {
                data = new[]
                {
                    new { id = "page-id_1", story = "Someone created an event" }
                }
            };
            string feedJson = JsonSerializer.Serialize(feed);

            string fbEventJson = JsonSerializer.Serialize(new
            {
                id = "1",
                name = "No Cover Event",
                description = "desc",
                place = (object?)null,
                start_time = DateTime.UtcNow.AddDays(1),
                end_time = DateTime.UtcNow.AddDays(1).AddHours(1),
                cover = (object?)null
            });

            var fbClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, feedJson, fbEventJson))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient("Facebook")).Returns(fbClient);

            // Default image client should not be called
            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK)) { BaseAddress = new Uri("http://localhost/") });

            var cut = new SyncFacebookEventsJob(_httpClientFactory.Object, _storesMock.Object, _jobService.Object, _logger.Object, _imageProcessor.Object);

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            createdEvent.Should().NotBeNull();
            _httpClientFactory.Verify(f => f.CreateClient(string.Empty), Times.Never);
            eventRepoMock.Verify(r => r.Create(It.IsAny<Event>()), Times.Once);
        }
    }
}