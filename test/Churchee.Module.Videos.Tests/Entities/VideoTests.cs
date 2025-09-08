using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Videos.Entities
{
    public class VideoTests
    {
        [Fact]
        public void DefaultConstructor_InitializesPropertiesToDefaults()
        {
            // Act
            var video = new Video();

            // Assert
            Assert.Equal(string.Empty, video.VideoUri);
            Assert.Equal(string.Empty, video.SourceName);
            Assert.Equal(string.Empty, video.ThumbnailUrl);
        }

        [Fact]
        public void ParameterizedConstructor_SetsAllPropertiesCorrectly()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            string videoUri = "https://example.com/video.mp4";
            var publishedDate = new DateTime(2024, 1, 1);
            string sourceName = "YouTube";
            string sourceId = "abc123";
            string title = "Test Video";
            string description = "A test video.";
            string thumbnailUrl = "https://example.com/thumb.jpg";
            string videosPath = "Videos";
            var pageTypeId = Guid.NewGuid();
            var before = DateTime.Now;

            // Act
            var video = new Video(applicationTenantId, videoUri, publishedDate, sourceName, sourceId, title, description, thumbnailUrl, videosPath, pageTypeId);
            var after = DateTime.Now;

            // Assert
            video.VideoUri.Should().Be(videoUri);
            video.PublishedDate.Should().Be(publishedDate);
            video.ThumbnailUrl.Should().Be(thumbnailUrl);
            video.SourceName.Should().Be(sourceName);
            video.SourceId.Should().Be(sourceId);
            video.SyncDate.Should().BeOnOrAfter(before);
            video.SyncDate.Should().BeOnOrBefore(after);
            video.IsSystem.Should().BeTrue();
            video.PageTypeId.Should().Be(pageTypeId);
            video.Url.Should().Be($"/{videosPath.ToLowerInvariant()}/{title.ToURL()}");
            video.Published.Should().BeTrue();
        }
    }
}
