using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Site.Entities;
using FluentAssertions;
using Hangfire;
using Moq;

namespace Churchee.Module.Events.Tests.Features.Commands.CreateEvent
{
    public class CreateEventCommandHandlerTests
    {
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
            DateTime startDate = DateTime.Now.AddDays(2);
            DateTime endDate = DateTime.Now.AddDays(3);
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


        private static IDataStore GetDataStore()
        {
            var mockPageRepository = new Mock<IRepository<Page>>();
            var mockPageTypeRepository = new Mock<IRepository<PageType>>();
            var mockViewTemplateRepository = new Mock<IRepository<ViewTemplate>>();
            var mockEventRepository = new Mock<IRepository<Event>>();

            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<PageType>()).Returns(mockPageTypeRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<Page>()).Returns(mockPageRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<Event>()).Returns(mockEventRepository.Object);

            return mockDataStore.Object;
        }
    }
}
