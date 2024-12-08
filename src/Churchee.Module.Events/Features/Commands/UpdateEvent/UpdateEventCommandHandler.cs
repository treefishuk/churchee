using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using Hangfire;
using MediatR;

namespace Churchee.Module.Events.Features.Commands
{
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, CommandResponse>
    {

        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IBlobStore _blobStore;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public UpdateEventCommandHandler(IDataStore dataStore, ICurrentUser currentUser, IBlobStore blobStore, IBackgroundJobClient backgroundJobClient)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
            _blobStore = blobStore;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<CommandResponse> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var entity = await _dataStore.GetRepository<Event>().FirstOrDefaultAsync(new EventByIdIncludingDatesSpecification(request.Id), cancellationToken);

            entity.UpdateInfo(title: request.Title,
                                   description: request.Description,
                                   content: request.Content,
                                   locationName: request.LocationName,
                                   city: request.City,
                                   street: request.Street,
                                   postCode: request.PostCode,
                                   country: request.Country,
                                   latitude: request.Latitude,
                                   longitude: request.Longitude);

            UpdateEventDates(request, entity);

            string imagePath = await UpdateImageAndReturnPath(request, applicationTenantId, cancellationToken);

            if (!string.IsNullOrEmpty(imagePath))
            {
                entity.SetImageUrl(imagePath);
            }

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private static void UpdateEventDates(UpdateEventCommand request, Event eventEntity)
        {
            //Clear all dates if empty
            if (request.Dates.Count == 0)
            {
                eventEntity.EventDates.Clear();

                return;
            }

            var existingDates = eventEntity.EventDates;

            //Add new Dates
            foreach (var date in request.Dates)
            {
                if (!existingDates.Any(a => a.Id == date.Id))
                {
                    eventEntity.AddDate(date.Id, date.Start, date.End);
                }
            }

            //Remove Deleted Dates
            foreach (var date in existingDates)
            {
                if (!request.Dates.Exists(a => a.Id == date.Id))
                {
                    Event.RemoveDate(date);
                }
            }
        }

        private async Task<string> UpdateImageAndReturnPath(UpdateEventCommand request, Guid applicationTenantId, CancellationToken cancellationToken)
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

                var bytes = ms.ConvertStreamToByteArray();

                _backgroundJobClient.Enqueue<ImageCropsGenerator>(x => x.CreateCrops(applicationTenantId, finalImagePath, bytes, true));

                return finalImagePath;
            }

            return imagePath;
        }
    }
}
