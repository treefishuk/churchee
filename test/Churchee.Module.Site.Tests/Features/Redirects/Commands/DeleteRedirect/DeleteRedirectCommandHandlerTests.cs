using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Redirects.Commands;
using Moq;

public class DeleteRedirectCommandHandlerTests
{
    [Fact]
    public async Task Handle_DeletesRedirectAndSavesChanges()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<RedirectUrl>>();
        int redirectId = 42;
        storageMock.Setup(x => x.GetRepository<RedirectUrl>()).Returns(repoMock.Object);
        var handler = new DeleteRedirectCommandHandler(storageMock.Object);
        var command = new DeleteRedirectCommand(redirectId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(x => x.SoftDelete(redirectId), Times.Once);
        storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<CommandResponse>(result);
    }
}
