using Xunit;
using Churchee.Module.Videos.Features.Commands;

namespace Churchee.Module.Videos.Features.Commands
{
    public class VideosEnabledCommandTests
    {
        [Fact]
        public void Constructor_SetsPageNameForVideos()
        {
            // Arrange
            var pageName = "videos";

            // Act
            var command = new VideosEnabledCommand(pageName);

            // Assert
            Assert.Equal(pageName, command.PageNameForVideos);
        }
    }
}
