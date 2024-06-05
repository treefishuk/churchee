using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
