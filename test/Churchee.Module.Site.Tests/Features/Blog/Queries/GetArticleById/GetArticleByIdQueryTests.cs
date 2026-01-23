using Churchee.Module.Site.Features.Blog.Queries;

namespace Churchee.Module.Site.Tests.Features.Blog.Queries.GetArticleById
{
    public class GetArticleByIdQueryTests
    {
        [Fact]
        public void Constructor_SetsArticleId()
        {
            // Arrange
            var articleId = Guid.NewGuid();

            // Act
            var query = new GetArticleByIdQuery(articleId);

            // Assert
            Assert.Equal(articleId, query.ArticleId);
        }
    }
}
