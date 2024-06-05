using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Podcasts.Anchor.Features.Podcasts.Commands
{
    public class EnableAnchorPodcastSyncCommand : IRequest<CommandResponse>
    {
        public EnableAnchorPodcastSyncCommand(string anchorFMRSSFeed)
        {
            AnchorFMRSSFeed = anchorFMRSSFeed;
        }

        public string AnchorFMRSSFeed { get; private set; }

    }
}
