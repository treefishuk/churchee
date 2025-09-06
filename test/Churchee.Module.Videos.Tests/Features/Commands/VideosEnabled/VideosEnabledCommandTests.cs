using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Videos.Features.Commands
{
    public class VideosEnabledCommandTests
    {
        [Fact]
        public void Constructor_SetsPageNameForVideos()
        {
            // Arrange
            string pageName = "videos";

            // Act
            var command = new VideosEnabledCommand(pageName);

            // Assert
            command.PageNameForVideos.Should().Be(pageName);
        }
    }
}
