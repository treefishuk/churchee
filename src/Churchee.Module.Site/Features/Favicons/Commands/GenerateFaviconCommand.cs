using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Favicons.Commands
{
    public class GenerateFaviconCommand : IRequest<CommandResponse>
    {
        public GenerateFaviconCommand(string base64Content, Guid folderId)
        {
            Base64Content = base64Content;
            FolderId = folderId;
        }

        public string Base64Content { get; set; }

        public Guid FolderId { get; set; }

    }
}
