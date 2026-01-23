using Churchee.Module.Events.Features.Commands;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Events.Tests.Features.Commands.ActivateEvents
{
    public class ActivateEventsCommandTests
    {
        [Fact]
        public void ActivateEventsCommand_PropertiesSetByConstructor()
        {
            //arrange
            var applicationTenantId = Guid.NewGuid();

            //act
            var command = new ActivateEventsCommand(applicationTenantId);

            //assert
            command.ApplicationTenantId.Should().Be(applicationTenantId);
        }
    }
}
