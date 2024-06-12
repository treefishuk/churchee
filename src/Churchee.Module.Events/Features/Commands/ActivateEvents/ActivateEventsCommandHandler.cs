using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Tenancy.Entities;
using MediatR;
using Page = Churchee.Module.Site.Entities.Page;

namespace Churchee.Module.Events.Features.Commands
{
    public class ActivateEventsCommandHandler : IRequestHandler<ActivateEventsCommand, CommandResponse>
    {

        private readonly ISettingStore _settingStore;
        private readonly IDataStore _dataStore;

        public ActivateEventsCommandHandler(ISettingStore settingStore, IDataStore dataStore)
        {
            _settingStore = settingStore;
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(ActivateEventsCommand request, CancellationToken cancellationToken)
        {
            var tenant = _dataStore.GetRepository<ApplicationTenant>().GetById(request.ApplicationTenantId);

            Guid eventListingPageTypeId = Guid.NewGuid();

            Guid eventDetailsPageTypeId = Guid.NewGuid();

            await CreatePageTypes(request.ApplicationTenantId, eventListingPageTypeId, eventDetailsPageTypeId);

            await CreateEventsListingPage(request, tenant, eventListingPageTypeId);

            await CreateViewTemplates(request.ApplicationTenantId);

            return new CommandResponse();
        }

        private async Task CreateEventsListingPage(ActivateEventsCommand request, ApplicationTenant tenant, Guid eventListingPageTypeId)
        {
            var pageRepo = _dataStore.GetRepository<Page>();

            if (!pageRepo.GetQueryable().Any(a => a.Url == "/events"))
            {
                var newListingPage = new Page(request.ApplicationTenantId, "Events", "/events", $"Upcomging Events for {tenant.Name}", eventListingPageTypeId, null, false);

                pageRepo.Create(newListingPage);

                await _dataStore.SaveChangesAsync();
            }
        }

        private async Task CreatePageTypes(Guid applicationTenantId, Guid eventListingPageTypeId, Guid eventDetailsPageTypeId)
        {
            var pageTypeRepo = _dataStore.GetRepository<PageType>();

            var alreadyExists = pageTypeRepo.AnyWithFiltersDisabled(w => w.SystemKey == PageTypes.EventDetailPageTypeId && w.ApplicationTenantId == applicationTenantId);

            if (!alreadyExists)
            {
                var newDetailPageType = new PageType(eventDetailsPageTypeId, PageTypes.EventDetailPageTypeId, applicationTenantId, false, "Event Detail", false);

                var newListingPageType = new PageType(eventListingPageTypeId, PageTypes.EventListingPageTypeId, applicationTenantId, true, "Event Listing", false);

                newListingPageType.AddChildType(newDetailPageType);

                pageTypeRepo.Create(newDetailPageType);

                pageTypeRepo.Create(newListingPageType);

                await _dataStore.SaveChangesAsync();
            }
        }

        private async Task CreateViewTemplates(Guid applicationTenantId)
        {
            var viewTemplatesRepo = _dataStore.GetRepository<ViewTemplate>();

            if (!viewTemplatesRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/EventListing.cshtml"))
            {
                var eventsListing = new ViewTemplate(applicationTenantId, "/Views/Shared/EventListing.cshtml", "");

                eventsListing.SetContent($@"

                    @{{
                        ViewData[""Title""] = Model.Title;                        
                        var page = ViewBag.Page; 
                    }}

                    <section>

                        <div class=""container mb-5"">
                            <h1>Events</h1>
                            @await Component.InvokeAsync(""EventListing"", new {{page = page}})
                        </div>

                    </section>

                ");

                viewTemplatesRepo.Create(eventsListing);

            }

            if (!viewTemplatesRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/EventDetail.cshtml"))
            {

                var eventDetail = new ViewTemplate(applicationTenantId, "/Views/Shared/EventDetail.cshtml", "");

                eventDetail.SetContent($@"

                    @{{
                        ViewData[""Title""] = Model.Title;
                    }}

                    <section>

                        <div class=""container mb-5"">
                            @await Component.InvokeAsync(""EventDetail"", new {{path = Context.Request.Path.ToString()}})
                        </div>

                    </section>


@section scripts{{

 <script>

    var simpleMap = document.getElementById(""simple-map"");

    if(simpleMap != null)
    {{
        var latitude = simpleMap.getAttribute(""data-latitude"");
        var longitude = simpleMap.getAttribute(""data-latitude"");

        var map = L.map('simple-map').setView([latitude, longitude], 16);

        L.tileLayer('https://tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png?{{foo}}', {{ foo: 'bar', attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors' }}).addTo(map);

        var marker = L.marker([latitude, longitude]).addTo(map);

    }}

</script>

}}


                ");

                viewTemplatesRepo.Create(eventDetail);

            }

            await _dataStore.SaveChangesAsync();
        }

    }
}
