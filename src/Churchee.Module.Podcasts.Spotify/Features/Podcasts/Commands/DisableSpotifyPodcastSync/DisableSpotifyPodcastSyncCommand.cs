using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands
{
    public class DisableSpotifyPodcastSyncCommand : IRequest<CommandResponse>
    {
    }
}
