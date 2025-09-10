using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Media.Commands;
using Moq;

public class CreateMediaFolderCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesMediaFolderAndSavesChanges()
    {
        // Arrange
        var currentUserMock = new Mock<ICurrentUser>();
        var dataStoreMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<MediaFolder>>();
        var tenantId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var parentFolder = new MediaFolder(tenantId, "Parent", string.Empty);

        currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);
        dataStoreMock.Setup(x => x.GetRepository<MediaFolder>()).Returns(repoMock.Object);
        repoMock.Setup(x => x.GetByIdAsync(parentId, It.IsAny<CancellationToken>())).ReturnsAsync(parentFolder);

        var handler = new CreateMediaFolderCommandHandler(currentUserMock.Object, dataStoreMock.Object);
        var command = new CreateMediaFolderCommand(parentId, "NewFolder");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(x => x.Create(It.IsAny<MediaFolder>()), Times.Once);
        dataStoreMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<CommandResponse>(result);
    }
}
