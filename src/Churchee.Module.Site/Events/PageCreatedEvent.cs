using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Events
{
    public class PageCreatedEvent : INotification
    {
        public PageCreatedEvent(Guid pageId, Guid pageTypeId)
        {
            PageId = pageId;
            PageTypeId = pageTypeId;
        }

        public Guid PageId { get; private set; }

        public Guid PageTypeId { get; private set; }

    }
}
