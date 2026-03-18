using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Google.Reviews.Features.Commands
{
    public class SyncNowCommand : IRequest<CommandResponse>
    {
    }
}
