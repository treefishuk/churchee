using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Commands.CreatePage;
using Moq;

public class CreatePageCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesPageAndSavesChanges()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var currentUserMock = new Mock<ICurrentUser>();
        var repoMock = new Mock<IRepository<Page>>();
        var tenantId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var parentUrl = "/parent";
        var title = "Test Page";
        var description = "desc";
        var pageTypeId = Guid.NewGuid();

        currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);
        storageMock.Setup(x => x.GetRepository<Page>()).Returns(repoMock.Object);
        repoMock.Setup(x => x.GetQueryable()).Returns(new[] { new Page(tenantId, "parent", parentUrl, "", pageTypeId, null, true) { Id = parentId, Url = parentUrl } }.AsQueryable());

        var handler = new CreatePageCommandHandler(storageMock.Object, currentUserMock.Object);
        var command = new CreatePageCommand(title, description, pageTypeId.ToString(), parentId.ToString());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(x => x.Create(It.IsAny<Page>()), Times.Once);
        storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<CommandResponse>(result);
    }
}
