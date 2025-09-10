using Churchee.Module.Site.Features.Media.Commands;
using FluentValidation.TestHelper;

public class CreateMediaFolderCommandValidatorTests
{
    [Fact]
    public void Should_Have_Error_When_ParentId_Is_Null()
    {
        // Arrange
        var validator = new CreateMediaFolderCommandValidator();
        var command = new CreateMediaFolderCommand(null, "Folder");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ParentId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ParentId_Is_Not_Null()
    {
        // Arrange
        var validator = new CreateMediaFolderCommandValidator();
        var command = new CreateMediaFolderCommand(Guid.NewGuid(), "Folder");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.ParentId);
    }
}
