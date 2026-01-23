using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Events;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.EventHandlers
{
    public class AddPageContentTypesToNewPage : INotificationHandler<PageCreatedEvent>
    {

        private readonly IDataStore _storage;

        public AddPageContentTypesToNewPage(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task Handle(PageCreatedEvent notification, CancellationToken cancellationToken)
        {
            var pageTypes = await _storage.GetRepository<PageType>().FirstOrDefaultAsync(new PageTypeWithPageTypeContentSpecification(notification.PageTypeId), cancellationToken);

            foreach (var content in pageTypes.PageTypeContent)
            {
                _storage.GetRepository<PageContent>().Create(new PageContent(content.Id, notification.PageId, string.Empty, 1));
            }

            await _storage.SaveChangesAsync(cancellationToken);
        }
    }
}
