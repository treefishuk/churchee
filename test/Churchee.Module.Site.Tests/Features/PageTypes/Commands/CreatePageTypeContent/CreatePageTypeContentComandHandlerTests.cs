using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.PageTypes.Commands.CreatePageTypeContent;
using Churchee.Module.Site.Specifications;
using Moq;

namespace Churchee.Module.Site.Tests.Features.PageTypes.Commands.CreatePageTypeContent
{
    public class CreatePageTypeContentCommandHandlerTests
    {

        [Fact]
        public async Task Handle_AddsPageTypeContentAndSavesChanges()
        {
            // Arrange
            var storageMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<PageType>>();
            var pageTypeId = Guid.NewGuid();
            var pageType = new PageType(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), true, "Test");

            repoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPageTypeByIdAndIncludePageTypeContentSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pageType);

            storageMock.Setup(x => x.GetRepository<PageType>()).Returns(repoMock.Object);

            var handler = new CreatePageTypeContentCommandHandler(storageMock.Object);
            var command = new CreatePageTypeContentCommand(pageTypeId, "Name", "string", true, 1);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<CommandResponse>(result);
            storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Optionally, verify that the PageTypeContent was added to the PageType
            Assert.Contains(pageType.PageTypeContent, c => c.Name == "Name" && c.Type == "string" && c.IsRequired && c.Order == 1);
        }
    }
}
