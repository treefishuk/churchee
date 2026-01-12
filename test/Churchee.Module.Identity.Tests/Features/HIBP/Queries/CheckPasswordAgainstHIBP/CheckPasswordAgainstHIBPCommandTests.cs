using Churchee.Module.Identity.Features.HIBP.Queries;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Identity.Tests.Features.HIBP.Queries.CheckPasswordAgainstHIBP
{
    public class CheckPasswordAgainstHibpCommandTests
    {
        [Fact]
        public void Constructor_ShouldInitializePasswordProperty()
        {
            // Arrange
            var password = "TestPassword";

            // Act
            var command = new CheckPasswordAgainstHibpCommand(password);

            // Assert
            command.Password.Should().Be(password);
        }

        [Fact]
        public void Constructor_ShouldInitializeNullPasswordProperty_WhenPasswordIsNull()
        {

            // Act
            var command = new CheckPasswordAgainstHibpCommand(null);

            // Assert
            command.Password.Should().BeNull();
        }

        [Fact]
        public void Constructor_ShouldInitializeEmptyPasswordProperty_WhenPasswordIsEmpty()
        {
            // Act
            var command = new CheckPasswordAgainstHibpCommand(string.Empty);

            // Assert
            command.Password.Should().Be(string.Empty);
        }
    }

}
