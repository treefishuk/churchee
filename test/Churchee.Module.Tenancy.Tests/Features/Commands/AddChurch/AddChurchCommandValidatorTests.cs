using Churchee.Common.Abstractions.Auth;
using Churchee.Module.Tenancy.Features.Churches.Commands;
using Churchee.Module.Tenancy.Features.Churches.Commands.AddChurch;
using FluentValidation.TestHelper;
using Moq;

namespace Churchee.Module.Tenancy.Tests.Features.Commands.AddChurch
{
    public class AddChurchCommandValidatorTests
    {
        [Fact]
        public void Should_Have_Error_When_Not_SysAdmin()
        {
            // Arrange
            string name = "Test Church";
            int charityNumber = 123456;
            var command = new AddChurchCommand(name, charityNumber);

            var currentUserMock = new Mock<ICurrentUser>();
            currentUserMock.Setup(s => s.HasRole("SysAdmin")).Returns(false);

            var validator = new AddChurchCommandValidator(currentUserMock.Object);

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_SysAdmin()
        {
            // Arrange
            string name = "Test Church";
            int charityNumber = 123456;
            var command = new AddChurchCommand(name, charityNumber);

            var currentUserMock = new Mock<ICurrentUser>();
            currentUserMock.Setup(s => s.HasRole("SysAdmin")).Returns(true);

            var validator = new AddChurchCommandValidator(currentUserMock.Object);

            var result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}
