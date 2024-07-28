using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Queries
{
    internal class GetPodcastSettingsRequest : IRequest<GetPodcastSettingsResponse>
    {
    }
}
