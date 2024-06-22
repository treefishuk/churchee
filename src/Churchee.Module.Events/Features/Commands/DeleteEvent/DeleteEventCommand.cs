using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Events.Features.Commands
{
    public class DeleteEventCommand : IRequest<CommandResponse>
    {
        public Guid EventId { get; set; }

        public DeleteEventCommand(Guid eventId)
        {
            EventId = eventId;
        }
    }
}
