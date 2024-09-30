using Churchee.Module.Events.Features.Commands;
using FluentAssertions;

namespace Churchee.Module.Events.Tests.Features.Commands.DeleteEvent
{
    public class DeleteEventCommandTests
    {
        [Fact]
        public void DeleteEventCommand_ParametersSet()
        {
            //arrange
            var eventId = Guid.NewGuid();

            //act
            var cut = new DeleteEventCommand(eventId);

            //assert
            cut.EventId.Should().Be(eventId);
        }

    }
}
