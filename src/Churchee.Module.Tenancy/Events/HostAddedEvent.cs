using MediatR;

namespace Churchee.Module.Tenancy.Events
{
    public class HostAddedEvent : INotification
    {
        public HostAddedEvent(string host, Guid applicationTenantId)
        {
            ApplicationTenantId = applicationTenantId;
            Host = host;
        }

        public Guid ApplicationTenantId { get; private set; }

        public string Host { get; private set; }
    }
}
