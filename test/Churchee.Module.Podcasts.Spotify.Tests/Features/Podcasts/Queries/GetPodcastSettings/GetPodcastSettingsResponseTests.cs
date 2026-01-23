using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Queries;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Podcasts.Spotify.Tests.Features.Podcasts.Queries.GetPodcastSettings
{
    public class GetPodcastSettingsResponseTests
    {
        [Fact]
        public void GetPodcastSettingsResponse_ShouldReturnCorrectValues()
        {
            // Arrange
            var spotifyUrl = "https://www.spotify.com";
            var nameForContent = "Spotify";
            var lastRun = DateTime.Now;

            // Act
            var response = new GetPodcastSettingsResponse(spotifyUrl, nameForContent, lastRun);

            // Assert
            response.SpotifyUrl.Should().Be(spotifyUrl);
            response.NameForContent.Should().Be(nameForContent);
            response.LastRun.Should().Be(lastRun);
        }
    }
}
