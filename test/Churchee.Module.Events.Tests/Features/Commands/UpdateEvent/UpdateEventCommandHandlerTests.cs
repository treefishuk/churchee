using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Events.Models;
using Churchee.Module.Events.Specifications;
using FluentAssertions;
using Hangfire;
using Moq;

namespace Churchee.Module.Events.Tests.Features.Commands.UpdateEvent
{
    public class UpdateEventCommandHandlerTests
    {
        [Fact]
        public async Task UpdateEventCommandHandler_Handler_DataSetAndCommandResponseReturned()
        {
            //arrange
            var mockCurrentUser = new Mock<ICurrentUser>();
            var mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            var testEvent = GetTestEvent();
            var command = GetCommand();

            //act
            var cut = new UpdateEventCommandHandler(GetDataStore(testEvent), mockCurrentUser.Object, GetBlobStore(), mockBackgroundJobClient.Object);
            var result = await cut.Handle(command, default);

            //assert
            testEvent.Title.Should().Be(command.Title);
            testEvent.Description.Should().Be(command.Description);
            testEvent.Content.Should().Be(command.Content);
            testEvent.ImageUrl.Should().Be("/img/events/event-img.jpg");
            testEvent.LocationName.Should().Be(command.LocationName);
            testEvent.City.Should().Be(command.City);
            testEvent.Street.Should().Be(command.Street);
            testEvent.PostCode.Should().Be(command.PostCode);
            testEvent.Country.Should().Be(command.Country);
            testEvent.Latitude.Should().Be(command.Latitude);
            testEvent.Longitude.Should().Be(command.Longitude);
            testEvent.EventDates.Count.Should().Be(2);
            result.IsSuccess.Should().BeTrue();
        }

        private static IBlobStore GetBlobStore()
        {
            var mockBlobStore = new Mock<IBlobStore>();

            mockBlobStore.Setup(s => s.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync("/img/events/event-img.jpg");

            return mockBlobStore.Object;
        }

        private static IDataStore GetDataStore(Event testevent)
        {
            var mockEventRepository = new Mock<IRepository<Event>>();
            mockEventRepository.Setup(s => s.FirstOrDefaultAsync(It.IsAny<EventByIdIncludingDatesSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(testevent);

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(s => s.GetRepository<Event>()).Returns(mockEventRepository.Object);
            return mockDataStore.Object;
        }

        private Event GetTestEvent()
        {
            var testEvent = new Event(Guid.NewGuid(), Guid.NewGuid(), "/events", Guid.NewGuid(), "", "", "Test Event", "", "", "", "", "", "", "", null, null, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1), "");

            return testEvent;
        }

        private UpdateEventCommand GetCommand()
        {
            Guid id = Guid.NewGuid();
            string title = "Updated Title";
            string description = "Longer more descriptive text";
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
            List<EventDateModel> dates =
            [
                new EventDateModel(DateTime.Now, DateTime.Now.AddMinutes(90))
            ];

            return new UpdateEventCommand(
                id: id,
                title: title,
                description: description,
                content: content,
                imageFileName: imageFileName,
                base64Image: base64Image,
                locationName: locationName,
                city: city,
                street: street,
                postCode: postcode,
                country: country,
                latitude: latitude,
                longitude: longitude,
                dates: dates);

        }

    }
}
