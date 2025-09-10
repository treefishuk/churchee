using Churchee.Module.Site.Features.Blog.Commands;

namespace Churchee.Module.Site.Tests.Features.Blog.Commands.CreateArticle
{
    public class CreateArticleCommandTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var command = new CreateArticleCommand();
            var title = "Test Title";
            var description = "Test Description";
            var content = "Test Content";
            var publishOnDate = DateTime.UtcNow;
            var parentPageId = Guid.NewGuid();

            // Act
            command.Title = title;
            command.Description = description;
            command.Content = content;
            command.PublishOnDate = publishOnDate;
            command.ParentPageId = parentPageId;

            // Assert
            Assert.Equal(title, command.Title);
            Assert.Equal(description, command.Description);
            Assert.Equal(content, command.Content);
            Assert.Equal(publishOnDate, command.PublishOnDate);
            Assert.Equal(parentPageId, command.ParentPageId);
        }
    }
}
