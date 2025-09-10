using Churchee.Module.Site.Features.Blog.Commands;

namespace Churchee.Module.Site.Tests.Features.Blog.Commands
{
    public class DeleteArticleCommandTests
    {
        [Fact]
        public void Constructor_SetsArticleId()
        {
            // Arrange
            var articleId = Guid.NewGuid();

            // Act
            var command = new DeleteArticleCommand(articleId);

            // Assert
            Assert.Equal(articleId, command.ArticleId);
        }
    }
}
