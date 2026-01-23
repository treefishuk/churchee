using Churchee.Module.Site.Features.Blog.Commands;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Site.Tests.Features.Blog.Commands.UnPublishArticle
{
    public class UnPublishArticleCommandTests
    {
        [Fact]
        public void UnPublishArticleCommand_Has_Correct_ArticleId_Property()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            // Act
            var command = new UnPublishArticleCommand(articleId);
            // Assert
            command.ArticleId.Should().Be(articleId);
        }

    }
}
