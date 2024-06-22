using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Events.Features.Commands
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CommandResponse>
    {

        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IBlobStore _blobStore;
        private readonly IImageProcessor _imageProcessor;

        public CreateEventCommandHandler(IDataStore dataStore, ICurrentUser currentUser, IBlobStore blobStore, IImageProcessor imageProcessor)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
            _blobStore = blobStore;
            _imageProcessor = imageProcessor;
        }

        public async Task<CommandResponse> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string imagePath = await CreateImageAndReturnPath(request, applicationTenantId, cancellationToken);

            Guid pageTypeId = _dataStore.GetRepository<PageType>()
                .ApplySpecification(new PageTypeFromSystemKeySpecification(PageTypes.EventDetailPageTypeId))
                .Select(s => s.Id)
                .FirstOrDefault();

            var repo = _dataStore.GetRepository<Event>();

            var newEvent = new Event(
                applicationTenantId: applicationTenantId,
                pageTypeId: pageTypeId,
                sourceName: "Churchee",
                sourceId: "N/A",
                title: request.Title,
                description: request.Description,
                content: request.Content,
                locationName: request.LocationName,
                city: request.City,
                street: request.Street,
                postCode: request.PostCode,
                country: request.Country,
                latitude: request.Latitude,
                longitude: request.Longitude,
                start: request.Start,
                end: request.End,
                imageUrl: imagePath);

            repo.Create(newEvent);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
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

                return await _blobStore.SaveAsync(applicationTenantId, imagePath, ms, false, true, cancellationToken);

            }

            return imagePath;
        }
    }
}
