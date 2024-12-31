using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands;

namespace Churchee.Module.Podcasts.Spotify.Tests.Features.Podcasts.Commands.EnableSpotifyPodcastSync
{
    public class EnableSpotifyPodcastSyncCommandTests
    {
        [Fact]
        public void EnableSpotifyPodcastSyncCommand_Properties_Initialised()
        {
            //arrange
            string rssFeed = "http://example.com/rss";

            //act
            var cut = new EnableSpotifyPodcastSyncCommand(rssFeed);

            //assert
            Assert.Equal(rssFeed, cut.SpotifyFMRSSFeed);
        }
    }
}
