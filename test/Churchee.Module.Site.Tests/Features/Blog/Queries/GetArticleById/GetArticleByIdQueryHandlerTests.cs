using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Queries;
using Churchee.Module.Site.Features.Blog.Queries.GetArticleById;
using Moq;

public class GetArticleByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsArticleResponse()
    {
        // Arrange
        var dataStoreMock = new Mock<IDataStore>();
        var repoMock = new Mock<IRepository<Article>>();
        var articleId = Guid.NewGuid();
        var expectedResponse = new GetArticleByIdResponse
        {
            Id = articleId,
            Title = "Test Title",
            Description = "desc",
            Content = "content",
            CreatedAt = DateTime.UtcNow,
            IsPublished = true,
            PublishOnDate = DateTime.UtcNow,
            ParentName = "Parent",
            ParentId = Guid.NewGuid()
        };
        dataStoreMock.Setup(x => x.GetRepository<Article>()).Returns(repoMock.Object);
        repoMock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<Article>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<Article, GetArticleByIdResponse>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        var handler = new GetArticleByIdQueryHandler(dataStoreMock.Object);
        var query = new GetArticleByIdQuery(articleId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Title, result.Title);
    }
}
