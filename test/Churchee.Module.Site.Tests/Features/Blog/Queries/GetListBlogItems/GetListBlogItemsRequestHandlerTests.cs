using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Queries.GetListBlogItems;
using Moq;

public class GetListBlogItemsRequestHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsDataTableResponse()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<Article>>();
        var articles = new List<Article>
        {
            new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title1", "/url1", "desc1") { Published = true },
            new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title2", "/url2", "desc2") { Published = false }
        }.AsQueryable();
        repoMock.Setup(x => x.Count()).Returns(articles.Count());
        repoMock.Setup(x => x.GetQueryable()).Returns(articles);
        storageMock.Setup(x => x.GetRepository<Article>()).Returns(repoMock.Object);
        var handler = new GetListBlogItemsRequestHandler(storageMock.Object);
        var request = (GetListBlogItemsRequest)System.Activator.CreateInstance(
            typeof(GetListBlogItemsRequest),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { 0, 10, "", "Title" },
            null);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.RecordsTotal);
        Assert.Equal(2, result.Data.Count());
    }
}
