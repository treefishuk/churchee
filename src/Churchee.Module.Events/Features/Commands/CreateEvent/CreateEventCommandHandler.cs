using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;
using Hangfire;
using MediatR;

namespace Churchee.Module.Events.Features.Commands
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CommandResponse>
    {

        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IBlobStore _blobStore;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CreateEventCommandHandler(IDataStore dataStore, ICurrentUser currentUser, IBlobStore blobStore, IBackgroundJobClient backgroundJobClient)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
            _blobStore = blobStore;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<CommandResponse> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string imagePath = await CreateImageAndReturnPath(request, applicationTenantId, cancellationToken);

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
                .SetImageUrl(imagePath)
                .SetPublished(true)
                .Build();

            AddUniqueSufficIfNeeded(newEvent);

            repo.Create(newEvent);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private void AddUniqueSufficIfNeeded(Event newEvent)
        {
            var repo = _dataStore.GetRepository<Event>();

            // Ensure unique URL by adding/incrementing a numeric suffix if needed
            string baseUrl = newEvent.Url;
            string uniqueUrl = baseUrl;
            int suffix = 1;

            while (repo.GetQueryable().Any(a => a.Url == uniqueUrl))
            {
                // If baseUrl already ends with -number, increment it
                int lastDash = baseUrl.LastIndexOf('-');
                if (lastDash > 0 && int.TryParse(baseUrl[(lastDash + 1)..], out int existingNumber))
                {
                    baseUrl = baseUrl[..lastDash];
                    suffix = existingNumber + 1;
                }
                uniqueUrl = $"{baseUrl}-{suffix}";
                suffix++;
            }

            newEvent.Url = uniqueUrl;
        }

        private async Task<string> CreateImageAndReturnPath(CreateEventCommand request, Guid applicationTenantId, CancellationToken cancellationToken)
        {

            string imagePath = string.Empty;

            if (!string.IsNullOrEmpty(request.ImageFileName))
            {
                string extension = Path.GetExtension(request.ImageFileName);

                string fileName = Path.GetFileNameWithoutExtension(request.ImageFileName);

                byte[] data = Convert.FromBase64String(request.Base64Image.Split(',')[1]);

                using var ms = new MemoryStream(data);

                imagePath = $"/img/events/{fileName.ToDevName()}{extension}";

                string finalImagePath = await _blobStore.SaveAsync(applicationTenantId, imagePath, ms, false, cancellationToken);

                byte[] bytes = ms.ConvertStreamToByteArray();

                _backgroundJobClient.Enqueue<ImageCropsGenerator>(x => x.CreateCrops(applicationTenantId, finalImagePath, bytes, true));

                return finalImagePath;
            }

            return imagePath;
        }
    }
}
