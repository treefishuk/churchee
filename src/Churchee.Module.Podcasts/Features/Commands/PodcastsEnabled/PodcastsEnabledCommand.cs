using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Podcasts.Features.Commands
{
    public class PodcastsEnabledCommand : IRequest<CommandResponse>
    {
        public PodcastsEnabledCommand(string pageNameForPodcasts)
        {
            PageNameForPodcasts = pageNameForPodcasts;
        }

        public string PageNameForPodcasts { get; }

    }
}
