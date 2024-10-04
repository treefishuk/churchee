using Churchee.Module.Events.Models;
using FluentAssertions;

namespace Churchee.Module.Events.Tests.Models
{
    public class EventDateModelTests
    {
        [Fact]
        public void EventDateModel_NewConstructor_SetsProperties()
        {
            //arrange

            var startDate = DateTime.Now.AddHours(24);
            var endDate = DateTime.Now.AddHours(26);

            //act
            var cut = new EventDateModel(startDate, endDate);

            //assert
            cut.Id.Should().NotBe(Guid.Empty);
            cut.Start.Should().Be(startDate);
            cut.End.Should().Be(endDate);
        }

        [Fact]
        public void EventDateModel_ExistingConstructor_SetsProperties()
        {
            //arrange
            var id = Guid.NewGuid();
            var startDate = DateTime.Now.AddHours(24);
            var endDate = DateTime.Now.AddHours(26);

            //act
            var cut = new EventDateModel(id, startDate, endDate);

            //assert
            cut.Id.Should().Be(id);
            cut.Start.Should().Be(startDate);
            cut.End.Should().Be(endDate);
        }

    }
}
