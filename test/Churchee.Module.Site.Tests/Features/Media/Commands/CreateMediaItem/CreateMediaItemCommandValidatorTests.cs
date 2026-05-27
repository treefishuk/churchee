using Churchee.Common.Storage;
using Churchee.Module.Site.Features.Media.Commands;
using FluentValidation.TestHelper;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Media.Commands.CreateMediaItem
{
    public class CreateMediaItemCommandValidatorTests
    {

        private readonly Mock<IDataStore> _mockDataStore;

        public CreateMediaItemCommandValidatorTests()
        {
            _mockDataStore = new Mock<IDataStore>();
        }


        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var validator = new CreateMediaItemCommandValidator(_mockDataStore.Object);
            var command = new CreateMediaItemCommand { Name = "" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);

        }

        [Fact]
        public void Should_Have_Error_When_FileName_Has_Invalid_Characters()
        {
            var validator = new CreateMediaItemCommandValidator(_mockDataStore.Object);
            var command = new CreateMediaItemCommand { FileName = "invalid name!" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_FileName_Is_Valid()
        {
            var validator = new CreateMediaItemCommandValidator(_mockDataStore.Object);
            var command = new CreateMediaItemCommand { FileName = "valid_name-123" };
            var result = validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.FileName);
        }
    }
}
