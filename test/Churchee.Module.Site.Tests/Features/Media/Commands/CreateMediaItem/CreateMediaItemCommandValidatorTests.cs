using Churchee.Module.Site.Features.Media.Commands;
using FluentValidation.TestHelper;

namespace Churchee.Module.Site.Tests.Features.Media.Commands.CreateMediaItem
{
    public class CreateMediaItemCommandValidatorTests
    {
        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Empty()
        {
            var validator = new CreateMediaItemCommandValidator();
            var command = new CreateMediaItemCommand { Name = "" };
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task Should_Have_Error_When_FileName_Has_Invalid_Characters()
        {
            var validator = new CreateMediaItemCommandValidator();
            var command = new CreateMediaItemCommand { FileName = "invalid name!" };
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_FileName_Is_Valid()
        {
            var validator = new CreateMediaItemCommandValidator();
            var command = new CreateMediaItemCommand { FileName = "valid_name-123" };
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveValidationErrorFor(x => x.FileName);
        }
    }
}
