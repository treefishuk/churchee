using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.YouTube.Features.YouTube.Commands
{
    public class DisableYouTubeSyncCommand : IRequest<CommandResponse>
    {
    }
}
