using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Events;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.EventHandlers
{
    public class AddNewPageContentTypesToExistingPages : INotificationHandler<PageTypeContentCreatedEvent>
    {

        private readonly IDataStore _storage;

        public AddNewPageContentTypesToExistingPages(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task Handle(PageTypeContentCreatedEvent notification, CancellationToken cancellationToken)
        {
            var pages = _storage.GetRepository<Page>()
                .ApplySpecification(new PagesWithContentThatImplementPageTypeSpecification(notification.PageTypeId));

            foreach (var page in pages)
            {
                page.AddContent(notification.PageTypeContentId, page.Id, "", page.Version);
            }

            await _storage.SaveChangesAsync(cancellationToken);
        }


    }
}
