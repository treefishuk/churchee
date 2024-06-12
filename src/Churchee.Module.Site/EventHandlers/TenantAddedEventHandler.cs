using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Events;
using MediatR;

namespace Churchee.Module.Site.EventHandlers
{
    public class TenantAddedEventHandler : INotificationHandler<TenantAddedEvent>
    {

        private readonly IDataStore _storage;

        public TenantAddedEventHandler(IDataStore store)
        {
            _storage = store;
        }

        public async Task Handle(TenantAddedEvent notification, CancellationToken cancellationToken)
        {
            var applicationTenantId = notification.ApplicationTenantId;

            var pageTypeId = await CreateHomePagePageType(applicationTenantId, cancellationToken);

            await CreateHomePage(applicationTenantId, pageTypeId, cancellationToken);
        }

        private async Task CreateHomePage(Guid applicationTenantId, Guid pageTypeId, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<Page>();

            var newPage = new Page(applicationTenantId, "Home", "/", "Home Page", pageTypeId, null, false);

            repo.Create(newPage);
                
            await _storage.SaveChangesAsync(cancellationToken);
        }

        private async Task<Guid> CreateHomePagePageType(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<PageType>();

            var newPageType = new PageType(Guid.NewGuid(), Guid.NewGuid(), applicationTenantId, true, "Home");

            repo.Create(newPageType);

            await _storage.SaveChangesAsync(cancellationToken);

            return newPageType.Id;
        }
    }
}
