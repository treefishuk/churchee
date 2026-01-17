using Churchee.Module.Site.Features.Blog.Commands;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Site.Tests.Features.Blog.Commands.PublishArticle
{
    public class PublishArticleCommandTests
    {
        [Fact]
        public void PublishArticleCommand_Has_Correct_ArticleId_Property()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            // Act
            var command = new PublishArticleCommand(articleId);
            // Assert
            command.ArticleId.Should().Be(articleId);
        }
    }
}
