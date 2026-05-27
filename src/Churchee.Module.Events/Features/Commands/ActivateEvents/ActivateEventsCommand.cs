using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Events.Features.Commands
{
    public class ActivateEventsCommand : IRequest<CommandResponse>
    {
        public ActivateEventsCommand(Guid applicationTenantId)
        {
            ApplicationTenantId = applicationTenantId;
        }

        public Guid ApplicationTenantId { get; }

    }
}
