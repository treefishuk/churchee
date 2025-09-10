using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Queries;
using Moq;
using Xunit;

public class GetListingQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsListingItems()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<Page>>();
        var parentId = Guid.NewGuid();
        var pages = new List<Page>
        {
            new Page(Guid.NewGuid(), "Title1", "/url1", "desc1", Guid.NewGuid(), null, true) { Id = Guid.NewGuid(), ParentId = parentId },
            new Page(Guid.NewGuid(), "Title2", "/url2", "desc2", Guid.NewGuid(), parentId, true) { Id = Guid.NewGuid(), ParentId = parentId }
        }.AsQueryable();
        repoMock.Setup(x => x.GetQueryable()).Returns(pages);
        storageMock.Setup(x => x.GetRepository<Page>()).Returns(repoMock.Object);
        var handler = new GetListingQueryHandler(storageMock.Object);
        var query = new GetListingQuery(parentId, "Title");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, item => Assert.Contains("Title", item.Title));
    }
}
