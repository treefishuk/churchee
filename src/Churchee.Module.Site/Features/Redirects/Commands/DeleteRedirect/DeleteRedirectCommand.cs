using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.Redirects.Commands
{
    public class DeleteRedirectCommand : IRequest<CommandResponse>
    {
        public DeleteRedirectCommand(int redirectId)
        {
            RedirectId = redirectId;
        }

        public int RedirectId { get; private set; }

    }
}
