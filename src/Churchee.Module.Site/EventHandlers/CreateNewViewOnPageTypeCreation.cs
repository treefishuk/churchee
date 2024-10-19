using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Events;
using MediatR;

namespace Churchee.Module.Site.EventHandlers
{
    public class CreateNewViewOnPageTypeCreation : INotificationHandler<PageTypeCreatedEvent>
    {

        private readonly IDataStore _storage;

        public CreateNewViewOnPageTypeCreation(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task Handle(PageTypeCreatedEvent notification, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<ViewTemplate>();

            var exists = repo.AnyWithFiltersDisabled(a => a.Location == $"/Views/Shared/{notification.Name.ToPascalCase()}.cshtml");

            if (exists)
            {
                return;
            }

            repo.Create(new ViewTemplate(notification.ApplicationTenantId, $"/Views/Shared/{notification.Name.ToPascalCase()}.cshtml", EmptyViewTemplate));

            await _storage.SaveChangesAsync(cancellationToken);
        }

        private const string EmptyViewTemplate = $@"

            @model Churchee.Sites.Models.Page

        ";
    }
}
