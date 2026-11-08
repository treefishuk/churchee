using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Events;
using Churchee.Module.Site.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.EventHandlers
{
    public class UpdateSlugsOnPageInfoUpdateEvent : INotificationHandler<PageInfoUpdatedEvent>
    {

        private readonly IDataStore _storage;

        public UpdateSlugsOnPageInfoUpdateEvent(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task Handle(PageInfoUpdatedEvent notification, CancellationToken cancellationToken)
        {
            string slug = await UpdatePageUrl(notification, cancellationToken);

            await UpdateChildPageUrls(slug, notification.PageId);

            await _storage.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateChildPageUrls(string parentSlug, Guid parentId)
        {
            var repo = _storage.GetRepository<WebContent>();

            var children = await repo.GetQueryable().Where(w => w.ParentId == parentId).ToListAsync();

            if (children.Count > 0)
            {
                foreach (var child in children)
                {
                    child.Url = $"{parentSlug}/{child.Title.ToURL()}";
                }
            }
        }

        private async Task<string> UpdatePageUrl(PageInfoUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<WebContent>();

            var page = await repo.FirstOrDefaultAsync(new WebContentFromIdAndTenantSpecification(notification.ApplicationTenantId, notification.PageId), cancellationToken);

            if (page.Url == "/")
            {
                return "/";
            }

            string parentSlug = string.Empty;

            if (page.ParentId != null)
            {
                parentSlug = await repo.GetQueryable().Where(w => w.Id == page.ParentId).Select(s => s.Url).FirstOrDefaultAsync(cancellationToken);
            }

            string slug = $"{parentSlug}/{page.Title.ToURL()}";

            page.Url = slug;

            return slug;
        }
    }
}
