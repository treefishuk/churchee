using Churchee.Module.Site.Features.Media.Commands;

public class DeleteMediaItemCommandTests
{
    [Fact]
    public void Constructor_SetsMediaItemId()
    {
        // Arrange
        var mediaItemId = Guid.NewGuid();

        // Act
        var command = new DeleteMediaItemCommand(mediaItemId);

        // Assert
        Assert.Equal(mediaItemId, command.MediaItemId);
    }
}
