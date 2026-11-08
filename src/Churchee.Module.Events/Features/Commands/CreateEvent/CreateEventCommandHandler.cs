using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Events.Features.Commands
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CommandResponse>
    {

        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public CreateEventCommandHandler(IDataStore dataStore, ICurrentUser currentUser, IBlobStore blobStore)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var pageTypeId = _dataStore.GetRepository<PageType>()
                .ApplySpecification(new PageTypeFromSystemKeySpecification(PageTypes.EventDetailPageTypeId, applicationTenantId))
                .Select(s => s.Id)
                .FirstOrDefault();

            string parentSlug = "/events";

            Guid? parentId = null;

            var parentPage = _dataStore.GetRepository<Page>().ApplySpecification(new EventListingPageSpecification()).FirstOrDefault();

            if (parentPage != null)
            {

                parentSlug = parentPage.Url;
                parentId = parentPage.Id;
            }

            var repo = _dataStore.GetRepository<Event>();

            var newEvent = new Event.Builder()
                .SetApplicationTenantId(applicationTenantId)
                .SetParentId(parentId)
                .SetParentSlug(parentSlug)
                .SetPageTypeId(pageTypeId)
                .SetSourceName("Churchee")
                .SetSourceId("N/A")
                .SetTitle(request.Title)
                .SetDescription(request.Description)
                .SetContent(request.Content)
                .SetLocationName(request.LocationName)
                .SetCity(request.City)
                .SetStreet(request.Street)
                .SetPostCode(request.PostCode)
                .SetCountry(request.Country)
                .SetLatitude(request.Latitude)
                .SetLongitude(request.Longitude)
                .SetDates(request.Start, request.End)
                .SetPublished(string.IsNullOrEmpty(request.ImageFileName))
                .Build();

            SuffixGeneration.AddUniqueSuffixIfNeeded(newEvent, _dataStore.GetRepository<Event>());

            newEvent.SetImageInfo(string.IsNullOrEmpty(request.ImageFileName) ? string.Empty : request.ImageFileName, request.ImageAltTag);

            repo.Create(newEvent);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
