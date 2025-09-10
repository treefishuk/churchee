using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Commands;
using Churchee.Module.Site.Features.Pages.Commands.UpdatePage;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Pages.Commands.DeletePage
{
    public class DeletePageCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DeletesPageAndSavesChanges()
        {
            // Arrange
            var storageMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<Page>>();
            var pageId = Guid.NewGuid();
            storageMock.Setup(x => x.GetRepository<Page>()).Returns(repoMock.Object);
            var handler = new DeletePageCommandHandler(storageMock.Object);
            var command = new DeletePageCommand(pageId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(x => x.SoftDelete(pageId), Times.Once);
            storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
