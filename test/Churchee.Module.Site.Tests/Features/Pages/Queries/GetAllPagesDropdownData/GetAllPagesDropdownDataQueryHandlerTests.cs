using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.UI.Models;
using Moq;

public class GetAllPagesDropdownDataQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsDropdownInputs()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<Page>>();
        var dropdowns = new List<DropdownInput>
        {
            new DropdownInput { Title = "Page1", Value = Guid.NewGuid().ToString() },
            new DropdownInput { Title = "Page2", Value = Guid.NewGuid().ToString() }
        };
        storageMock.Setup(x => x.GetRepository<Page>()).Returns(repoMock.Object);
        repoMock.Setup(x => x.GetListAsync(
            It.IsAny<object>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<Page, DropdownInput>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(dropdowns);
        var handler = new GetAllPagesDropdownDataQueryHandler(storageMock.Object);
        var query = new GetAllPagesDropdownDataQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Collection(result,
            item => Assert.Equal("Page1", item.Title),
            item => Assert.Equal("Page2", item.Title));
    }
}
