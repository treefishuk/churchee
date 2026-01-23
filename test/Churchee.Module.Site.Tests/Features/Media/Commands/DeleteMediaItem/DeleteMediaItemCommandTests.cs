using Churchee.Module.Site.Features.Media.Commands;

namespace Churchee.Module.Site.Tests.Features.Media.Commands.DeleteMediaItem
{
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
}