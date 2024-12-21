using Churchee.Module.Events.Features.Commands;
using FluentAssertions;

namespace Churchee.Module.Events.Tests.Features.Commands.CreateEvent
{
    public class CreateEventCommandTests
    {
        [Fact]
        public void CreateEventCommand_PropertiesSetCorrectly()
        {
            //arrange
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

            //act
            var cut = new CreateEventCommand.Builder()
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

            //assert
            cut.Title.Should().Be(title);
            cut.Description.Should().Be(description);
            cut.Start.Should().Be(startDate);
            cut.End.Should().Be(endDate);
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
        }
    }
}
