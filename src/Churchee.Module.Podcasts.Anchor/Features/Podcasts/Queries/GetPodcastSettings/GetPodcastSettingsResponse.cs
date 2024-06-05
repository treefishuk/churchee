using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Podcasts.Anchor.Features.Podcasts.Queries
{
    public class GetPodcastSettingsResponse
    {
        public GetPodcastSettingsResponse(string anchorUrl, string nameForContent, DateTime? lastRun)
        {
            AnchorUrl = anchorUrl;
            NameForContent = nameForContent;
            LastRun = lastRun;
        }

        public string AnchorUrl { get; private set; }

        public string NameForContent { get; private set; }

        public DateTime? LastRun { get; private set; }

    }
}
