using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Videos.Features.Commands
{
    public class VideosEnabledCommand : IRequest<CommandResponse>
    {
        public VideosEnabledCommand(string pageNameForVideos)
        {
            PageNameForVideos = pageNameForVideos;
        }

        public string PageNameForVideos { get; }

    }
}
