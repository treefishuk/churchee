using Churchee.Module.Site.Features.Media.Commands;

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
