using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.YouTube.Features.YouTube.Commands.SyncNow
{
    public class SyncNowCommand : IRequest<CommandResponse>
    {
    }
}
