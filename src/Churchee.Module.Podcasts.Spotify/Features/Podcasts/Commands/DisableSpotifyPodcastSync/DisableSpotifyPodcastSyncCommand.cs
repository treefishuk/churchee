using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands
{
    public class DisableSpotifyPodcastSyncCommand : IRequest<CommandResponse>
    {
    }
}
