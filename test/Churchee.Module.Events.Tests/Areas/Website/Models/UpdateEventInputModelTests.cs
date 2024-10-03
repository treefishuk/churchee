using Churchee.Module.Events.Areas.Website.Models;
using FluentAssertions;

namespace Churchee.Module.Events.Tests.Areas.Website.Models
{
    public class UpdateEventInputModelTests
    {
        [Fact]
        public void UpdateEventInputModel_DefaultConstructor_SetsProperties()
        {
            var cut = new UpdateEventInputModel();

            cut.Title.Should().BeEmpty();
            cut.Description.Should().BeEmpty();
            cut.LocationName.Should().BeEmpty();
            cut.City.Should().BeEmpty();
            cut.Street.Should().BeEmpty();
            cut.PostCode.Should().BeEmpty();
            cut.Country.Should().BeEmpty();
            cut.Content.Should().BeEmpty();
            cut.ImageUpload.Should().NotBeNull();
            cut.Latitude.Should().BeNull();
            cut.Longitude.Should().BeNull();
        }

    }
}
