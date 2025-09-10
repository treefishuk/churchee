using Churchee.Module.Site.Features.Pages.Commands;

namespace Churchee.Module.Site.Tests.Features.Pages.Commands.DeletePage
{
    public class DeletePageCommandTests
    {
        [Fact]
        public void Constructor_SetsPageId()
        {
            // Arrange
            var pageId = Guid.NewGuid();

            // Act
            var command = new DeletePageCommand(pageId);

            // Assert
            Assert.Equal(pageId, command.PageId);
        }
    }
}
