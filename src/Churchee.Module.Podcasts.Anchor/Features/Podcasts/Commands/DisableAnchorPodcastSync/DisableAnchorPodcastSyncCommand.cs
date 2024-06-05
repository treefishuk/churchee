using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Podcasts.Anchor.Features.Podcasts.Commands
{
    public class DisableAnchorPodcastSyncCommand : IRequest<CommandResponse>
    {
        public DisableAnchorPodcastSyncCommand()
        {
        }

    }
}
