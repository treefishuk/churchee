using Churchee.Module.Events.Areas.Website.Models;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Events.Tests.Areas.Website.Models
{
    public class CreateEventInputModelTests
    {

        [Fact]
        public void CreateEventInputModel_DefaultConstructor_SetsProperties()
        {
            var cut = new CreateEventInputModel();

            cut.Title.Should().BeEmpty();
            cut.Description.Should().BeEmpty();
            cut.LocationName.Should().BeEmpty();
            cut.City.Should().BeEmpty();
            cut.Street.Should().BeEmpty();
            cut.PostCode.Should().BeEmpty();
            cut.Country.Should().BeEmpty();
            cut.Content.Should().BeEmpty();
            cut.ImageUpload.Should().NotBeNull();
            cut.Start.Should().BeNull();
            cut.End.Should().BeNull();
            cut.Latitude.Should().BeNull();
            cut.Longitude.Should().BeNull();
        }
    }
}
