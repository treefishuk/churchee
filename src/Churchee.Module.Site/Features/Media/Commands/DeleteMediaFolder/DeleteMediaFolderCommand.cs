using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class DeleteMediaFolderCommand : IRequest<CommandResponse>
    {
        public DeleteMediaFolderCommand(Guid folderId)
        {
            FolderId = folderId;
        }

        public Guid FolderId { get; }
    }
}
