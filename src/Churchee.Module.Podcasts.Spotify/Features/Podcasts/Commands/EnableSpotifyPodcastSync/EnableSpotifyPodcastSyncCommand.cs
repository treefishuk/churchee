using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands
{
    public class EnableSpotifyPodcastSyncCommand : IRequest<CommandResponse>
    {
        public EnableSpotifyPodcastSyncCommand(string spotifyFMRSSFeed)
        {
            SpotifyFMRSSFeed = spotifyFMRSSFeed;
        }

        public string SpotifyFMRSSFeed { get; private set; }

    }
}
