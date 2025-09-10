using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.PageTypes.Commands;
using Moq;

public class DeletePageTypeCommandHandlerTests
{
    [Fact]
    public async Task Handle_DeletesPageTypeAndSavesChanges()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<PageType>>();
        var id = Guid.NewGuid();
        storageMock.Setup(x => x.GetRepository<PageType>()).Returns(repoMock.Object);
        var handler = new DeletePageTypeCommandHandler(storageMock.Object);
        var command = new DeletePageTypeCommand(id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(x => x.SoftDelete(id), Times.Once);
        storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<CommandResponse>(result);
    }
}
