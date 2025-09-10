using Churchee.Module.Site.Features.Favicons.Commands;

namespace Churchee.Module.Site.Tests.Features.Favicons
{
    public class GenerateFaviconCommandTests
    {

        [Fact]
        public void GenerateFaviconCommand_InitializesProperties()
        {
            // Arrange
            string base64Content = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA...";
            var folderId = Guid.NewGuid();
            // Act
            var command = new GenerateFaviconCommand(base64Content, folderId);
            // Assert
            Assert.Equal(base64Content, command.Base64Content);
            Assert.Equal(folderId, command.FolderId);
        }

    }
}
