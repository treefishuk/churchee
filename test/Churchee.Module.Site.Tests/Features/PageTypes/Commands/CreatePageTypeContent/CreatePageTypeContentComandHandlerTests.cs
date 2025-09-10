using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.PageTypes.Commands.CreatePageTypeContent;
using Churchee.Module.Site.Specifications;
using Moq;

public class CreatePageTypeContentComandHandlerTests
{
    [Fact]
    public async Task Handle_AddsPageTypeContentAndSavesChanges()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<PageType>>();
        var pageTypeId = Guid.NewGuid();
        var pageType = new PageType(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), true, "Test");
        repoMock.Setup(x => x.ApplySpecification(It.IsAny<IncludePageTypeContentSpecification>())).Returns(new[] { pageType }.AsQueryable());
        storageMock.Setup(x => x.GetRepository<PageType>()).Returns(repoMock.Object);
        var handler = new CreatePageTypeContentComandHandler(storageMock.Object);
        var command = new CreatePageTypeContentComand(pageTypeId, "Name", "string", true, 1);
        pageType.Id = pageTypeId;

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<CommandResponse>(result);
        storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
