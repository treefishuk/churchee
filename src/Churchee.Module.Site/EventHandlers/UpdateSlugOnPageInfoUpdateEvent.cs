using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.EventHandlers
{
    public class UpdateSlugOnPageInfoUpdateEvent : INotificationHandler<PageInfoUpdatedEvent>
    {

        private readonly IDataStore _storage;

        public UpdateSlugOnPageInfoUpdateEvent(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task Handle(PageInfoUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<Page>();

            var page = await repo.GetByIdAsync(notification.PageId);

            if (page.Url == "/") {

                return;
            }

            string parentSlug = string.Empty;

            if (page.ParentId != null)
            {
                parentSlug = await repo.GetQueryable().Where(w => w.Id == page.ParentId).Select(s => s.Url).FirstOrDefaultAsync();
            }

            page.Url = $"{parentSlug}/{page.Title.ToURL()}";

            await _storage.SaveChangesAsync(cancellationToken);
        }
    }
}
