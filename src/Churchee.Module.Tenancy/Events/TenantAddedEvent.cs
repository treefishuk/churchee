using MediatR;

namespace Churchee.Module.Tenancy.Events
{
    public class TenantAddedEvent : INotification
    {
        public TenantAddedEvent(Guid applicationTenantId)
        {
            ApplicationTenantId = applicationTenantId;
        }

        public Guid ApplicationTenantId { get; private set; }
    }
}
