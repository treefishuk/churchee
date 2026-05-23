using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

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
