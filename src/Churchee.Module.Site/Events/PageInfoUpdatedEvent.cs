using MediatR;

namespace Churchee.Module.Site.Events
{
    public class PageInfoUpdatedEvent : INotification
    {
        public PageInfoUpdatedEvent(Guid pageId)
        {
            PageId = pageId;
        }

        public Guid PageId { get; private set; }

    }
}
