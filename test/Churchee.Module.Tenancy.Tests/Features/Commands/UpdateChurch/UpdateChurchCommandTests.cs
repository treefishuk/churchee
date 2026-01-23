using Churchee.Module.Tenancy.Features.Churches.Commands;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Tenancy.Tests.Features.Commands.UpdateChurch
{
    public class UpdateChurchCommandTests
    {
        [Fact]
        public void UpdateChurchCommandShould_Set_Properties_Correctly()
        {
            // Arrange
            var churchId = Guid.NewGuid();
            int newCharityNumber = 654321;
            var domains = new List<string> { "updatedchurch.churchee.com" };

            // Act
            var command = new UpdateChurchCommand(churchId, newCharityNumber, domains);

            // Assert
            command.Id.Should().Be(churchId);
            command.CharityNumber.Should().Be(newCharityNumber);
            command.Domains.Should().Contain("updatedchurch.churchee.com");
        }
    }
}
