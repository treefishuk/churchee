using Churchee.Common.Abstractions;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Queries.GetListBlogItems;
using Churchee.Module.Site.Features.Blog.Responses;
using Churchee.Module.Site.Specifications;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Site.Tests.Features.Blog.Queries.GetListBlogItems
{
    public class GetListBlogItemsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsDataTableResponse()
        {
            // Arrange
            var storageMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<Article>>();

            var publishedArticle = new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title1", "/url1", "desc1");
            publishedArticle.Publish();

            var unPublishedArticle = new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title2", "/url2", "desc2");

            var articles = new List<Article>
            {
                publishedArticle,
                unPublishedArticle
            }.AsQueryable();

            // Setup Count
            repoMock.Setup(x => x.Count()).Returns(articles.Count());

            // Setup GetDataTableResponseAsync
            repoMock.Setup(x => x.GetDataTableResponseAsync(
                It.IsAny<AllArticlesSpecification>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<Article, GetListBlogItemsResponseItem>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((AllArticlesSpecification spec, string orderBy, string orderByDir, int skip, int take, Expression<Func<Article, GetListBlogItemsResponseItem>> selector, CancellationToken token) =>
                {
                    var data = articles.Select(selector).ToList();
                    return new DataTableResponse<GetListBlogItemsResponseItem>
                    {
                        RecordsTotal = data.Count,
                        Data = data
                    };
                });

            storageMock.Setup(x => x.GetRepository<Article>()).Returns(repoMock.Object);

            var handler = new GetListBlogItemsRequestHandler(storageMock.Object);

            var request = new GetListBlogItemsRequest(0, 10, "", "Title");

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.RecordsTotal);
            Assert.Equal(2, result.Data.Count());
            Assert.Contains(result.Data, x => x.Title == "Title1");
            Assert.Contains(result.Data, x => x.Title == "Title2");
        }
    }
}
