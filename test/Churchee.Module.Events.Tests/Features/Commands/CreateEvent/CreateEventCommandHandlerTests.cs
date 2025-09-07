using Ardalis.Specification;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Site.Entities;
using Churchee.Test.Helpers.Validation;
using Hangfire;
using Moq;

namespace Churchee.Module.Events.Tests.Features.Commands.CreateEvent
{
    public class CreateEventCommandHandlerTests
    {

        private readonly Mock<IRepository<Page>> _mockPageRepository = new();
        private readonly Mock<IRepository<PageType>> _mockPageTypeRepository = new();
        private readonly Mock<IRepository<Event>> _mockEventRepository = new();
        private readonly Mock<IBlobStore> _mockBlobStore = new();
        private readonly Mock<ICurrentUser> _mockCurrentUser = new();
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient = new();

        [Fact]
        public async Task CreateEventCommandHandler_Handle_ReturnsCommandResponse()
        {
            //arrange
            var mockCurrentUser = new Mock<ICurrentUser>();

            mockCurrentUser.Setup(s => s.GetApplicationTenantId()).ReturnsAsync(Guid.NewGuid());

            var mockBlobStore = new Mock<IBlobStore>();
            var mockBackgroundJobClient = new Mock<IBackgroundJobClient>();

            string title = "Text";
            string description = "Lomger more descriptive text";
            var startDate = DateTime.Now.AddDays(2);
            var endDate = DateTime.Now.AddDays(3);
            string content = "Some meaningful content";
            string imageFileName = "event-img.jpg";
            string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAANSURBVBhXY2BgYPgPAAEEAQBwIGULAAAAAElFTkSuQmCC";
            string locationName = "Somewhere";
            string city = "Emerald City";
            string street = "Yellow Brick Road";
            string postcode = "WW0 0ZZ";
            string country = "Oz";
            decimal latitude = 39.105092049099866m;
            decimal longitude = -94.62311716634233m;

            var command = new CreateEventCommand.Builder()
                .SetTitle(title)
                .SetDescription(description)
                .SetStart(startDate)
                .SetEnd(endDate)
                .SetContent(content)
                .SetImageFileName(imageFileName)
                .SetBase64Image(base64Image)
                .SetLocationName(locationName)
                .SetCity(city)
                .SetStreet(street)
                .SetPostCode(postcode)
                .SetCountry(country)
                .SetLatitude(latitude)
                .SetLongitude(longitude)
                .Build();

            //act
            var cut = new CreateEventCommandHandler(GetDataStore(), mockCurrentUser.Object, mockBlobStore.Object, mockBackgroundJobClient.Object);

            var result = await cut.Handle(command, default);

            //assert
            result.IsSuccess.Should().BeTrue();
        }


        [Fact]
        public async Task Handle_WhenUrlExists_AppendsSuffix()
        {
            // Arrange
            var existingEvents = new List<Event>
            {
                new() { Url = "/events/my-event" }
            };

            _mockEventRepository.Setup(r => r.GetQueryable()).Returns(existingEvents.AsQueryable());

            _mockPageRepository.Setup(x => x.ApplySpecification(It.IsAny<ISpecification<Page>>(), false)).Returns(Enumerable.Empty<Page>().AsQueryable());

            _mockCurrentUser.Setup(s => s.GetApplicationTenantId()).ReturnsAsync(Guid.NewGuid());

            var cut = new CreateEventCommandHandler(GetDataStore(), _mockCurrentUser.Object, _mockBlobStore.Object, _mockBackgroundJobClient.Object);

            string title = "/my-event";
            string description = "Lomger more descriptive text";
            var startDate = DateTime.Now.AddDays(2);
            var endDate = DateTime.Now.AddDays(3);
            string content = "Some meaningful content";
            string imageFileName = "event-img.jpg";
            string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAANSURBVBhXY2BgYPgPAAEEAQBwIGULAAAAAElFTkSuQmCC";
            string locationName = "Somewhere";
            string city = "Emerald City";
            string street = "Yellow Brick Road";
            string postcode = "WW0 0ZZ";
            string country = "Oz";
            decimal latitude = 39.105092049099866m;
            decimal longitude = -94.62311716634233m;

            var command = new CreateEventCommand.Builder()
                .SetTitle(title)
                .SetDescription(description)
                .SetStart(startDate)
                .SetEnd(endDate)
                .SetContent(content)
                .SetImageFileName(imageFileName)
                .SetBase64Image(base64Image)
                .SetLocationName(locationName)
                .SetCity(city)
                .SetStreet(street)
                .SetPostCode(postcode)
                .SetCountry(country)
                .SetLatitude(latitude)
                .SetLongitude(longitude)
                .Build();

            // Act
            await cut.Handle(command, CancellationToken.None);

            // Assert
            _mockEventRepository.Verify(r => r.Create(It.Is<Event>(e => e.Url == "/events/my-event-2")), Times.Once);
        }

        [Fact]

        public async Task Handle_WhenUrlExists_AppendsIncrementedSuffix()
        {
            // Arrange
            var existingEvents = new List<Event>
            {
                new() { Url = "/events/my-event" },
                new() { Url = "/events/my-event-1" }
            };

            _mockEventRepository.Setup(r => r.GetQueryable()).Returns(existingEvents.AsQueryable());

            _mockCurrentUser.Setup(s => s.GetApplicationTenantId()).ReturnsAsync(Guid.NewGuid());

            var cut = new CreateEventCommandHandler(GetDataStore(), _mockCurrentUser.Object, _mockBlobStore.Object, _mockBackgroundJobClient.Object);

            string title = "/my-event-1";
            string description = "Lomger more descriptive text";
            var startDate = DateTime.Now.AddDays(2);
            var endDate = DateTime.Now.AddDays(3);
            string content = "Some meaningful content";
            string imageFileName = "event-img.jpg";
            string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAANSURBVBhXY2BgYPgPAAEEAQBwIGULAAAAAElFTkSuQmCC";
            string locationName = "Somewhere";
            string city = "Emerald City";
            string street = "Yellow Brick Road";
            string postcode = "WW0 0ZZ";
            string country = "Oz";
            decimal latitude = 39.105092049099866m;
            decimal longitude = -94.62311716634233m;

            var command = new CreateEventCommand.Builder()
                .SetTitle(title)
                .SetDescription(description)
                .SetStart(startDate)
                .SetEnd(endDate)
                .SetContent(content)
                .SetImageFileName(imageFileName)
                .SetBase64Image(base64Image)
                .SetLocationName(locationName)
                .SetCity(city)
                .SetStreet(street)
                .SetPostCode(postcode)
                .SetCountry(country)
                .SetLatitude(latitude)
                .SetLongitude(longitude)
                .Build();

            // Act
            await cut.Handle(command, CancellationToken.None);

            // Assert
            _mockEventRepository.Verify(r => r.Create(It.Is<Event>(e => e.Url == "/events/my-event-2")), Times.Once);
        }

        private IDataStore GetDataStore()
        {

            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<PageType>()).Returns(_mockPageTypeRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<Page>()).Returns(_mockPageRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<Event>()).Returns(_mockEventRepository.Object);
            return mockDataStore.Object;
        }
    }
}
