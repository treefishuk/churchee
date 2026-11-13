using Churchee.Module.Podcasts.Entities;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Podcasts.Tests.Entities
{
    public class PodcastTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            string audioUri = "http://example.com/audio.mp3";
            var publishedDate = DateTime.UtcNow;
            string sourceName = "SourceName";
            string sourceId = "SourceId";
            string title = "Podcast Title";
            string description = "Podcast Description";
            string imageUrl = "http://example.com/image.jpg";
            string thumbnailUrl = "http://example.com/thumbnail.jpg";
            string podcastsUrl = "podcasts";
            var podcastDetailPageTypeId = Guid.NewGuid();

            // Act
            var podcast = new Podcast(applicationTenantId, audioUri, publishedDate, sourceName, sourceId, title, description, imageUrl, thumbnailUrl, podcastsUrl, podcastDetailPageTypeId);

            // Assert
            podcast.ApplicationTenantId.Should().Be(applicationTenantId);
            podcast.AudioUri.Should().Be(audioUri);
            podcast.PublishedDate.Should().Be(publishedDate);
            podcast.SyncDate.Date.Should().Be(DateTime.Now.Date);
            podcast.ImageUrl.Should().Be(imageUrl);
            podcast.ThumbnailUrl.Should().Be(thumbnailUrl);
            podcast.IsSystem.Should().BeTrue();
            podcast.PageTypeId.Should().Be(podcastDetailPageTypeId);
            podcast.Url.Should().Be($"/{podcastsUrl.ToLowerInvariant()}/{title.ToURL()}");
            podcast.SourceName.Should().Be(sourceName);
            podcast.SourceId.Should().Be(sourceId);
        }

        [Fact]
        public void DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var podcast = new Podcast();

            // Assert
            podcast.AudioUri.Should().BeEmpty();
            podcast.SourceName.Should().BeEmpty();
            podcast.ImageUrl.Should().BeEmpty();
            podcast.ThumbnailUrl.Should().BeEmpty();
        }
    }

}