using Churchee.Common.ResponseTypes;
using MediatR;

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
