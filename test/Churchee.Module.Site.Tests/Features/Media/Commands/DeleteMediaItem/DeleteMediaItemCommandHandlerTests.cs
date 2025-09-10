using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Media.Commands;
using Moq;

public class DeleteMediaItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_DeletesMediaItemAndSavesChanges()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<MediaItem>>();
        var mediaItemId = Guid.NewGuid();
        storageMock.Setup(x => x.GetRepository<MediaItem>()).Returns(repoMock.Object);
        var handler = new DeleteMediaItemCommandHandler(storageMock.Object);
        var command = new DeleteMediaItemCommand(mediaItemId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(x => x.SoftDelete(mediaItemId), Times.Once);
        storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<CommandResponse>(result);
    }
}
