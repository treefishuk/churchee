using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Events
{
    public class PageInfoUpdatedEvent : INotification
    {
        public PageInfoUpdatedEvent(Guid applicationTenantId, Guid pageId)
        {
            PageId = pageId;
            ApplicationTenantId = applicationTenantId;
        }

        public Guid PageId { get; private set; }
        public Guid ApplicationTenantId { get; private set; }

    }
}
