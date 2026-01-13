using Churchee.Module.Tenancy.Features.Churches.Commands;

namespace Churchee.Module.Tenancy.Tests.Features.Commands.AddChurch
{
    public class AddChurchCommandTests
    {
        [Fact]
        public void AddChurchCommand_Should_Set_Properties_Correctly()
        {
            // Arrange
            string name = "Test Church";
            int charityNumber = 123456;
            // Act
            var command = new AddChurchCommand(name, charityNumber);
            // Assert
            Assert.Equal(name, command.Name);
            Assert.Equal(charityNumber, command.CharityNumber);
        }

    }
}
