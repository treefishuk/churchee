using System;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Events;
using Churchee.Module.Site.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var page = await _storage.GetRepository<Page>().GetQueryable().Where(w => w.Id == notification.PageId).FirstOrDefaultAsync();

            var pageContentTypes = await _storage.GetRepository<PageType>()
                .GetQueryable()
                .Where(w => w.Id == notification.PageTypeId)
                .Select(s => s.PageTypeContent)
                .FirstOrDefaultAsync();

            foreach (var type in pageContentTypes)
            {
                _storage.GetRepository<PageContent>().Create(new PageContent(type.Id, notification.PageId, string.Empty, 1));
            }

            await _storage.SaveChangesAsync(cancellationToken);
        }


    }
}
