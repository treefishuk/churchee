using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Events.Models;
using FluentAssertions;

namespace Churchee.Module.Events.Tests.Features.Commands.UpdateEvent
{
    public class UpdateEventCommandTests
    {
        [Fact]
        public void UpdateEventCommand_PropertiesSet()
        {
            //arrange
            Guid id = Guid.NewGuid();
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
            List<EventDateModel> dates =
            [
                new EventDateModel(DateTime.Now, DateTime.Now.AddMinutes(90))
            ];

            //act
            var cut = new UpdateEventCommand(
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

            //assert
            cut.Id.Should().Be(id);
            cut.Title.Should().Be(title);
            cut.Description.Should().Be(description);
            cut.Content.Should().Be(content);
            cut.ImageFileName.Should().Be(imageFileName);
            cut.Base64Image.Should().Be(base64Image);
            cut.LocationName.Should().Be(locationName);
            cut.City.Should().Be(city);
            cut.Street.Should().Be(street);
            cut.PostCode.Should().Be(postcode);
            cut.Country.Should().Be(country);
            cut.Latitude.Should().Be(latitude);
            cut.Longitude.Should().Be(longitude);
            cut.Dates[0].Should().Be(dates[0]);
        }
    }
}
