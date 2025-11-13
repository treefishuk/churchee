using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Events.Features.Commands.DuplicateEvent
{
    public class DuplicateEventCommand : IRequest<CommandResponse>
    {
        public Guid OriginalEventId { get; set; }

        public DuplicateEventCommand(Guid originalEventId)
        {
            OriginalEventId = originalEventId;
        }
    }
}
