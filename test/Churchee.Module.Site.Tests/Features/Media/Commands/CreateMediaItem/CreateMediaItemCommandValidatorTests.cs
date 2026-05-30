using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Media.Commands;
using Churchee.Module.Site.Specifications;
using FluentValidation.TestHelper;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Site.Tests.Features.Media.Commands.CreateMediaItem
{
    public class CreateMediaItemCommandValidatorTests
    {

        private readonly Mock<IDataStore> _mockDataStore;

        public CreateMediaItemCommandValidatorTests()
        {
            _mockDataStore = new Mock<IDataStore>();

            var mockMediaFolderRepository = new Mock<IRepository<MediaFolder>>();

            mockMediaFolderRepository.Setup(s => s.FirstOrDefaultAsync(It.IsAny<MediaFolderByIdSpecification>(), It.IsAny<Expression<Func<MediaFolder, string>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(".jpg,.png");

            _mockDataStore.Setup(s => s.GetRepository<MediaFolder>()).Returns(mockMediaFolderRepository.Object);
        }

        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Empty()
        {
            var validator = new CreateMediaItemCommandValidator(_mockDataStore.Object);
            var command = new CreateMediaItemCommand { Name = "", FolderId = Guid.NewGuid(), FileExtension = ".jpg" };
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);

        }

        [Fact]
        public async Task Should_Have_Error_When_FileName_Has_Invalid_Characters()
        {
            var validator = new CreateMediaItemCommandValidator(_mockDataStore.Object);
            var command = new CreateMediaItemCommand { FileName = "invalid name!", FolderId = Guid.NewGuid(), FileExtension = ".jpg" };
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_FileName_Is_Valid()
        {
            var validator = new CreateMediaItemCommandValidator(_mockDataStore.Object);
            var command = new CreateMediaItemCommand { FileName = "valid_name-123", FolderId = Guid.NewGuid(), FileExtension = ".jpg" };
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveValidationErrorFor(x => x.FileName);
        }
    }
}
