using Churchee.Module.Podcasts.Entities;
using FluentAssertions;

namespace Churchee.Module.Podcasts.Tests.Entities
{
    public class PodcastTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            var audioUri = "http://example.com/audio.mp3";
            var publishedDate = DateTime.UtcNow;
            var sourceName = "SourceName";
            var sourceId = "SourceId";
            var title = "Podcast Title";
            var description = "Podcast Description";
            var imageUrl = "http://example.com/image.jpg";
            var thumbnailUrl = "http://example.com/thumbnail.jpg";
            var podcastsUrl = "podcasts";
            var podcastDetailPageTypeId = Guid.NewGuid();

            // Act
            var podcast = new Podcast(applicationTenantId, audioUri, publishedDate, sourceName, sourceId, title, description, imageUrl, thumbnailUrl, podcastsUrl, podcastDetailPageTypeId);

            // Assert
            podcast.ApplicationTenantId.Should().Be(applicationTenantId);
            podcast.AudioUri.Should().Be(audioUri);
            podcast.PublishedDate.Should().Be(publishedDate);
            podcast.SyncDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
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