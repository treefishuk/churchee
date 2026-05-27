using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Events.Features.Commands
{
    public class DeleteEventCommand : IRequest<CommandResponse>
    {
        public DeleteEventCommand(Guid eventId)
        {
            EventId = eventId;
        }

        public Guid EventId { get; }
    }
}
