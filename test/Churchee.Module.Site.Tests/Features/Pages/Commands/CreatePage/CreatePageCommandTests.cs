using Churchee.Module.Site.Features.Pages.Commands.CreatePage;

namespace Churchee.Module.Site.Tests.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommandTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly_WithParentId()
        {
            // Arrange
            string title = "Test Title";
            string description = "Test Description";
            string pageTypeId = Guid.NewGuid().ToString();
            string parentId = Guid.NewGuid().ToString();

            // Act
            var command = new CreatePageCommand(title, description, pageTypeId, parentId);

            // Assert
            Assert.Equal(title, command.Title);
            Assert.Equal(description, command.Description);
            Assert.Equal(Guid.Parse(pageTypeId), command.PageTypeId);
            Assert.Equal(Guid.Parse(parentId), command.ParentId);
        }

        [Fact]
        public void Constructor_SetsParentIdToNull_WhenParentIdIsEmpty()
        {
            // Arrange
            string title = "Test Title";
            string description = "Test Description";
            string pageTypeId = Guid.NewGuid().ToString();
            string parentId = "";

            // Act
            var command = new CreatePageCommand(title, description, pageTypeId, parentId);

            // Assert
            Assert.Null(command.ParentId);
        }
    }
}
