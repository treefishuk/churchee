using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Events;
using MediatR;

namespace Churchee.Module.Site.EventHandlers
{
    public class AddDefaultMediaFoldersForNewTenantHandler : INotificationHandler<TenantAddedEvent>
    {

        private readonly IDataStore _dataStore;

        public AddDefaultMediaFoldersForNewTenantHandler(IDataStore store)
        {
            _dataStore = store;
        }

        public async Task Handle(TenantAddedEvent notification, CancellationToken cancellationToken)
        {
            var applicationTenantId = notification.ApplicationTenantId;

            var repo = _dataStore.GetRepository<MediaFolder>();

            repo.Create(new MediaFolder(applicationTenantId, "Images", ".jpg, .jpeg, .png, .webp"));
            repo.Create(new MediaFolder(applicationTenantId, "Fonts", ".woff, .woff2"));

            await _dataStore.SaveChangesAsync(cancellationToken);
        }


    }
}
