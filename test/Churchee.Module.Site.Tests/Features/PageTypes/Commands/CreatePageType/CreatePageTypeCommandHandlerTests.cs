using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.PageTypes.Commands.CreatePageType;
using Moq;

public class CreatePageTypeCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesPageTypeAndSavesChanges()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var currentUserMock = new Mock<ICurrentUser>();
        var repoMock = new Mock<IRepository<PageType>>();
        var tenantId = Guid.NewGuid();
        currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);
        storageMock.Setup(x => x.GetRepository<PageType>()).Returns(repoMock.Object);
        var handler = new CreatePageTypeCommandHandler(storageMock.Object, currentUserMock.Object);
        var command = new CreatePageTypeCommand("TestPageType");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(x => x.Create(It.IsAny<PageType>()), Times.Once);
        storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<CommandResponse>(result);
    }
}
