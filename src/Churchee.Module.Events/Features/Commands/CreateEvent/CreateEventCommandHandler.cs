using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Jobs;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;
using Hangfire;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Events.Features.Commands
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CommandResponse>
    {

        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IBlobStore _blobStore;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IImageProcessor _imageProcessor;

        public CreateEventCommandHandler(IDataStore dataStore, ICurrentUser currentUser, IBlobStore blobStore, IBackgroundJobClient backgroundJobClient, IImageProcessor imageProcessor)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
            _blobStore = blobStore;
            _backgroundJobClient = backgroundJobClient;
            _imageProcessor = imageProcessor;
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

            repo.Create(newEvent);

            await _dataStore.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrEmpty(request.ImageFileName) && request.ImageData.Length > 0)
            {
                _backgroundJobClient.Enqueue<EventImageProcessingJob>(x => x.ProcessAsync(applicationTenantId, newEvent.Id, request.ImageFileName, request.ImageData, CancellationToken.None));
            }

            return new CommandResponse();
        }
    }
}
