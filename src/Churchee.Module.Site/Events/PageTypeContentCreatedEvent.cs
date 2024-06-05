using MediatR;

namespace Churchee.Module.Site.Events
{
    public class PageTypeContentCreatedEvent : INotification
    {
        public PageTypeContentCreatedEvent(Guid applicationTenantId, Guid pageTypeContentId, Guid pageTypeId)
        {
            ApplicationTenantId = applicationTenantId;
            PageTypeContentId = pageTypeContentId;
            PageTypeId = pageTypeId;
        }

        public Guid ApplicationTenantId { get; set; }

        public Guid PageTypeId { get; set; }

        public Guid PageTypeContentId { get; set; }

    }
}
