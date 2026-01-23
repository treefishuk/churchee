using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Facebook.Events.Features.Commands
{
    public class SyncFacebookEventsToEventsTableCommand : IRequest<CommandResponse>
    {
    }
}
