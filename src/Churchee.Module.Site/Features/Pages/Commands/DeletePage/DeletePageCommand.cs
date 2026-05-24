using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.Pages.Commands
{
    public class DeletePageCommand : IRequest<CommandResponse>
    {
        public DeletePageCommand(Guid pageId)
        {
            PageId = pageId;
        }

        public Guid PageId { get; private set; }

    }
}
