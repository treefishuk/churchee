using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Facebook.Events.Features.Commands
{
    public class DisableFacebookSyncCommand : IRequest<CommandResponse>
    {
    }
}
