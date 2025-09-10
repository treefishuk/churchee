using Churchee.Module.Site.Features.Media.Commands;

namespace Churchee.Module.Site.Tests.Features.Media.Commands.DeleteMediaFolder
{
    public class DeleteMediaFolderCommandTests
    {
        [Fact]
        public void Constructor_SetsFolderId()
        {
            // Arrange
            var folderId = Guid.NewGuid();

            // Act
            var command = new DeleteMediaFolderCommand(folderId);

            // Assert
            Assert.Equal(folderId, command.FolderId);
        }
    }
}
