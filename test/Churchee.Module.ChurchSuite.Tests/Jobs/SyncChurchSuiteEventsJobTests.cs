using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.ChurchSuite.API;
using Churchee.Module.ChurchSuite.Events.Specifications;
using Churchee.Module.ChurchSuite.Jobs;
using Churchee.Module.Events.Entities;
using Churchee.Module.Site.Entities;
using Churchee.Test.Helpers;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;

namespace Churchee.Module.ChurchSuite.Tests.Jobs
{
    public class SyncChurchSuiteEventsJobTests
    {

        private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
        private readonly Mock<ISettingStore> _settingStore = new();
        private readonly Mock<IDataStore> _dataStore = new();
        private readonly Mock<IBlobStore> _blobStore = new();
        private readonly Mock<ILogger<SyncChurchSuiteEventsJob>> _logger = new();
        private readonly Mock<IJobService> _jobService = new();
        private readonly Mock<IImageProcessor> _imageProcessor = new();
        private readonly Mock<IRepository<Event>> eventRepoMock = new();
        private readonly Mock<IRepository<EventDate>> eventDateRepoMock = new();
        private readonly Mock<IRepository<PageType>> pageTypeRepoMock = new();
        private readonly Mock<IRepository<Page>> pageRepoMock = new();

        private readonly Guid tenantId;

        public SyncChurchSuiteEventsJobTests()
        {

            tenantId = Guid.NewGuid();

            _dataStore.Setup(x => x.GetRepository<Event>()).Returns(eventRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<PageType>()).Returns(pageTypeRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Page>()).Returns(pageRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<EventDate>()).Returns(eventDateRepoMock.Object);
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("page-id");
            _logger.Setup(s => s.IsEnabled(LogLevel.Error)).Returns(true);

        }

        [Fact]
        public async Task GetFeedResult_With_Test_Api_Returns_Data()
        {
            // Arrange
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("https://demo.churchsuite.com/embed/calendar/json");

            _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var cut = new SyncChurchSuiteEventsJob(_httpClientFactory.Object, _settingStore.Object, _dataStore.Object, _blobStore.Object, _jobService.Object, _imageProcessor.Object, _logger.Object);

            var result = await cut.GetFeedResult(tenantId);

            result.Count().Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task GetGroupedData_With_Test_Api_Returns_Data()
        {
            // Arrange
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("https://demo.churchsuite.com/embed/calendar/json");

            _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var cut = new SyncChurchSuiteEventsJob(_httpClientFactory.Object, _settingStore.Object, _dataStore.Object, _blobStore.Object, _jobService.Object, _imageProcessor.Object, _logger.Object);

            var result = await cut.GetGroupedData(tenantId);

            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ExecuteAsync_Does_Nothing_When_Nothing_Returned()
        {
            // Arrange
            string json = JsonSerializer.Serialize(new List<ApiResponse>());

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, json))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new SyncChurchSuiteEventsJob(_httpClientFactory.Object, _settingStore.Object, _dataStore.Object, _blobStore.Object, _jobService.Object, _imageProcessor.Object, _logger.Object);

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            _dataStore.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Creates_SingleEvent_When_Two_Events_With_Same_Sequence()
        {
            // Arrange
            string json = JsonSerializer.Serialize(new List<ApiResponse>() {
                new ApiResponse
                {
                    Id = 1,
                    Name = "Test Event",
                    Sequence = 1234,
                    DatetimeStart = DateTime.UtcNow,
                    DatetimeEnd = DateTime.UtcNow.AddHours(1),
                    Description = "This is a test event",
                    Status = "confirmed",
                    PublicVisible = true,
                },
                new ApiResponse
                {
                    Id = 2,
                    Name = "Test Event",
                    Sequence = 1234,
                    DatetimeStart = DateTime.UtcNow.AddDays(1),
                    DatetimeEnd = DateTime.UtcNow.AddDays(1).AddHours(1),
                    Description = "This is a test event",
                    Status = "confirmed",
                    PublicVisible = true
                }

            });

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, json))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new SyncChurchSuiteEventsJob(_httpClientFactory.Object, _settingStore.Object, _dataStore.Object, _blobStore.Object, _jobService.Object, _imageProcessor.Object, _logger.Object);

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(r => r.Create(It.IsAny<Event>()), Times.Once);
            _dataStore.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Creates_Two_Events_When_Not_Same_Sequence()
        {
            // Arrange
            string json = JsonSerializer.Serialize(new List<ApiResponse>() {
                new ApiResponse
                {
                    Id = 1,
                    Name = "Test Event",
                    Sequence = 9876,
                    DatetimeStart = DateTime.UtcNow,
                    DatetimeEnd = DateTime.UtcNow.AddHours(1),
                    Description = "This is a test event",
                    Status = "confirmed",
                    PublicVisible = true,
                },
                new ApiResponse
                {
                    Id = 2,
                    Name = "Test Event",
                    Sequence = 1234,
                    DatetimeStart = DateTime.UtcNow.AddDays(1),
                    DatetimeEnd = DateTime.UtcNow.AddDays(1).AddHours(1),
                    Description = "This is a test event",
                    Status = "confirmed",
                    PublicVisible = true
                }

            });

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, json))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new SyncChurchSuiteEventsJob(_httpClientFactory.Object, _settingStore.Object, _dataStore.Object, _blobStore.Object, _jobService.Object, _imageProcessor.Object, _logger.Object);

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(r => r.Create(It.IsAny<Event>()), Times.Exactly(2));
            _dataStore.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        [Fact]
        public async Task ExecuteAsync_Adds_Event_Date_When_Already_Exists()
        {
            // Arrange
            string json = JsonSerializer.Serialize(new List<ApiResponse>() {
                new ApiResponse
                {
                    Id = 1,
                    Name = "Test Event",
                    Sequence = 9876,
                    DatetimeStart = DateTime.UtcNow.AddDays(1),
                    DatetimeEnd = DateTime.UtcNow.AddDays(1).AddHours(1),
                    Description = "This is a test event",
                    Status = "confirmed",
                    PublicVisible = true,
                }
            });

            var existingEvent = new Event.Builder()
                .SetSourceId("9876")
                .SetTitle("Existing Event")
                .SetDescription("This is an existing event")
                .SetDates(DateTime.UtcNow, DateTime.UtcNow.AddHours(1))
                .Build();

            eventRepoMock.Setup(s => s.FirstOrDefaultAsync(It.IsAny<GetEventByChurchSuiteSequenceSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingEvent);

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, json))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new SyncChurchSuiteEventsJob(_httpClientFactory.Object, _settingStore.Object, _dataStore.Object, _blobStore.Object, _jobService.Object, _imageProcessor.Object, _logger.Object);

            // Act
            await cut.ExecuteAsync(tenantId, CancellationToken.None);

            // Assert
            eventRepoMock.Verify(r => r.Create(It.IsAny<Event>()), Times.Never);
            _dataStore.Verify(v => v.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
