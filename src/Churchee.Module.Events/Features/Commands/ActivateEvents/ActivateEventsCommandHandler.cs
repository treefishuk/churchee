using Churchee.Common.Helpers;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Tenancy.Entities;
using Page = Churchee.Module.Site.Entities.Page;

namespace Churchee.Module.Events.Features.Commands
{
    public class ActivateEventsCommandHandler : IRequestHandler<ActivateEventsCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;

        public ActivateEventsCommandHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(ActivateEventsCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var tenant = await _dataStore.GetRepository<ApplicationTenant>().GetByIdAsync(request.ApplicationTenantId, cancellationToken);

            var eventListingPageTypeId = Guid.NewGuid();

            var eventDetailsPageTypeId = Guid.NewGuid();

            await CreatePageTypes(request.ApplicationTenantId, eventListingPageTypeId, eventDetailsPageTypeId);

            await CreateEventsListingPage(request, tenant, eventListingPageTypeId);

            await CreateViewTemplates(request.ApplicationTenantId);

            return new CommandResponse();
        }

        private async Task CreateEventsListingPage(ActivateEventsCommand request, ApplicationTenant tenant, Guid eventListingPageTypeId)
        {
            var pageRepo = _dataStore.GetRepository<Page>();

            bool alreadyExists = pageRepo.AnyWithFiltersDisabled(w => w.PageType.SystemKey == PageTypes.EventListingPageTypeId && w.ApplicationTenantId == request.ApplicationTenantId);

            if (alreadyExists)
            {
                return;
            }

            var newListingPage = new Page(request.ApplicationTenantId, "Events", "/events", $"Upcomging Events for {tenant.Name}", eventListingPageTypeId, null, false);

            pageRepo.Create(newListingPage);

            await _dataStore.SaveChangesAsync();
        }

        private async Task CreatePageTypes(Guid applicationTenantId, Guid eventListingPageTypeId, Guid eventDetailsPageTypeId)
        {
            var pageTypeRepo = _dataStore.GetRepository<PageType>();

            bool alreadyExists = pageTypeRepo.AnyWithFiltersDisabled(w => w.SystemKey == PageTypes.EventDetailPageTypeId && w.ApplicationTenantId == applicationTenantId);

            if (alreadyExists)
            {
                return;
            }

            var newDetailPageType = new PageType(eventDetailsPageTypeId, PageTypes.EventDetailPageTypeId, applicationTenantId, false, "Event Detail", false);

            var newListingPageType = new PageType(eventListingPageTypeId, PageTypes.EventListingPageTypeId, applicationTenantId, true, "Event Listing", false);

            newListingPageType.AddChildType(newDetailPageType);

            pageTypeRepo.Create(newDetailPageType);

            pageTypeRepo.Create(newListingPageType);

            await _dataStore.SaveChangesAsync();
        }

        private async Task CreateViewTemplates(Guid applicationTenantId)
        {
            var viewTemplatesRepo = _dataStore.GetRepository<ViewTemplate>();

            CreateEventListingViewTenmplate(applicationTenantId, viewTemplatesRepo);
            CreateEventDetailViewTemplate(applicationTenantId, viewTemplatesRepo);

            await _dataStore.SaveChangesAsync();
        }

        private static void CreateEventDetailViewTemplate(Guid applicationTenantId, Common.Abstractions.Storage.IRepository<ViewTemplate> viewTemplatesRepo)
        {
            if (viewTemplatesRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/EventDetail.cshtml"))
            {
                return;
            }

            var eventDetail = new ViewTemplate(applicationTenantId, "/Views/Shared/EventDetail.cshtml", EmbeddedResourceHelper.ReadTemplate("EventDetail"));

            viewTemplatesRepo.Create(eventDetail);

        }

        private static void CreateEventListingViewTenmplate(Guid applicationTenantId, Common.Abstractions.Storage.IRepository<ViewTemplate> viewTemplatesRepo)
        {
            if (viewTemplatesRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/EventListing.cshtml"))
            {
                return;
            }

            var eventsListing = new ViewTemplate(applicationTenantId, "/Views/Shared/EventListing.cshtml", EmbeddedResourceHelper.ReadTemplate("EventListing"));

            viewTemplatesRepo.Create(eventsListing);
        }

    }
}
