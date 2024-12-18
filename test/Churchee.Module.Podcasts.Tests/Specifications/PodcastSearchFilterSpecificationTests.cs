using Churchee.Module.Podcasts.Entities;
using Churchee.Module.Podcasts.Specifications;
using FluentAssertions;

namespace Churchee.Module.Podcasts.Tests.Specifications
{
    public class PodcastSearchFilterSpecificationTests
    {

        [Fact]
        public void PodcastSearchFilterSpecification_Should_Return_AllItems_When_NoSearchText()
        {
            // Arrange
            var podcasts = new List<Podcast>
            {
                new Podcast(Guid.NewGuid(), "url", DateTime.Now, "Spotify", string.Empty, "Title1", "Content1", string.Empty, string.Empty, string.Empty, Guid.NewGuid()),
                new Podcast(Guid.NewGuid(), "url", DateTime.Now, "Spotify", string.Empty, "Title2", "Content2", string.Empty, string.Empty, string.Empty, Guid.NewGuid()),
                new Podcast(Guid.NewGuid(), "url", DateTime.Now, "Spotify", string.Empty, "Title3", "Content3", string.Empty, string.Empty, string.Empty, Guid.NewGuid()),
                new Podcast(Guid.NewGuid(), "url", DateTime.Now, "Spotify", string.Empty, "Title4", "Content4", string.Empty, string.Empty, string.Empty, Guid.NewGuid()),

            };

            var spec = new PodcastSearchFilterSpecification(string.Empty);

            // Act
            var result = spec.Evaluate(podcasts);

            // Assert
            result.Count().Should().Be(4);
        }


        [Fact]
        public void PodcastSearchFilterSpecification_Should_Return_ItemsThatContainText_When_SearchTextProvided()
        {
            // Arrange
            var podcasts = new List<Podcast>
            {
                new Podcast(Guid.NewGuid(), "url", DateTime.Now, "Spotify", string.Empty, "Title1", "Content1", string.Empty, string.Empty, string.Empty, Guid.NewGuid()),
                new Podcast(Guid.NewGuid(), "url", DateTime.Now, "Spotify", string.Empty, "Title2", "Content2", string.Empty, string.Empty, string.Empty, Guid.NewGuid()),
                new Podcast(Guid.NewGuid(), "url", DateTime.Now, "Spotify", string.Empty, "Title3", "Content3", string.Empty, string.Empty, string.Empty, Guid.NewGuid()),
                new Podcast(Guid.NewGuid(), "url", DateTime.Now, "Spotify", string.Empty, "Title4", "Content4", string.Empty, string.Empty, string.Empty, Guid.NewGuid()),

            };

            var spec = new PodcastSearchFilterSpecification("Title1");

            // Act
            var result = spec.Evaluate(podcasts);

            // Assert
            result.Count().Should().Be(1);
            result.First().Title.Should().Be("Title1");
        }

    }


}

