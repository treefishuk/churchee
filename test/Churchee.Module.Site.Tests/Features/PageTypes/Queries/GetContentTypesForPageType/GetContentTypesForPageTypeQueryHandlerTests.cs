using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.PageTypes.Queries;
using Churchee.Module.Site.Features.PageTypes.Queries.GetPageOfPageTypeContent;
using Moq;

public class GetContentTypesForPageTypeQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsContentTypes()
    {
        // Arrange
        var storageMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<PageTypeContent>>();
        var pageTypeId = Guid.NewGuid();
        var expected = new List<GetContentTypesForPageTypeResponse>
        {
            new GetContentTypesForPageTypeResponse { Id = Guid.NewGuid(), Name = "Name1", DevName = "Dev1", Type = "string", Required = true, Order = 1 },
            new GetContentTypesForPageTypeResponse { Id = Guid.NewGuid(), Name = "Name2", DevName = "Dev2", Type = "int", Required = false, Order = 2 }
        };
        storageMock.Setup(x => x.GetRepository<PageTypeContent>()).Returns(repoMock.Object);
        repoMock.Setup(x => x.GetListAsync(
            It.IsAny<object>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<PageTypeContent, GetContentTypesForPageTypeResponse>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var handler = new GetContentTypesForPageTypeQueryHandler(storageMock.Object);
        var query = new GetContentTypesForPageTypeQuery(pageTypeId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Collection(result,
            item => Assert.Equal("Name1", item.Name),
            item => Assert.Equal("Name2", item.Name));
    }
}
