using Churchee.Common.Storage;
using Churchee.Module.Site.Features.Pages.Commands.CreatePage;
using FluentValidation.TestHelper;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommandValidatorTests
    {
        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var storageMock = new Mock<IDataStore>();
            var validator = new CreatePageCommandValidator(storageMock.Object);
            var command = new CreatePageCommand("", "desc", System.Guid.NewGuid().ToString(), "");
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Title_Is_Not_Empty()
        {
            var storageMock = new Mock<IDataStore>();
            var validator = new CreatePageCommandValidator(storageMock.Object);
            var command = new CreatePageCommand("Title", "desc", System.Guid.NewGuid().ToString(), "");
            var result = validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }
    }
}
