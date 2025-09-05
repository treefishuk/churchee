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
            Assert.Equal(videoUri, video.VideoUri);
            Assert.Equal(publishedDate, video.PublishedDate);
            Assert.Equal(thumbnailUrl, video.ThumbnailUrl);
            Assert.Equal(sourceName, video.SourceName);
            Assert.Equal(sourceId, video.SourceId);
            Assert.True(video.SyncDate >= before && video.SyncDate <= after);
            Assert.True(video.IsSystem);
            Assert.Equal(pageTypeId, video.PageTypeId);
            Assert.Equal($"/{videosPath.ToLowerInvariant()}/{title.ToURL()}", video.Url);
            Assert.True(video.Published);
        }
    }
}
