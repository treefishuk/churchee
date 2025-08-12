using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Redirects.Commands
{
    public class CreateRedirectCommand : IRequest<CommandResponse>
    {
        public CreateRedirectCommand(string path, Guid webContentId)
        {
            Path = path;
            WebContentId = webContentId;
        }

        public string Path { get; set; }
        public Guid WebContentId { get; set; }

    }
}
