using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;

namespace Churchee.Module.Events.Features.Commands
{
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, CommandResponse>
    {

        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public UpdateEventCommandHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dataStore.GetRepository<Event>().FirstOrDefaultAsync(new EventByIdIncludingDatesSpecification(request.Id), cancellationToken);

            entity.SetImageInfo(string.IsNullOrEmpty(request.ImageFileName) ? entity.ImageUrl : request.ImageFileName, request.ImageAltTag);

            entity.UpdateInformation(title: request.Title,
                                   description: request.Description,
                                   content: request.Content);

            entity.UpdateLocation(locationName: request.LocationName,
                                   city: request.City,
                                   street: request.Street,
                                   postCode: request.PostCode,
                                   country: request.Country,
                                   latitude: request.Latitude,
                                   longitude: request.Longitude);

            UpdateEventDates(request, entity);

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
            foreach (var date in request.Dates.Where(w => !existingDates.Any(a => a.Id == w.Id)))
            {
                eventEntity.AddDate(date.Id, date.Start, date.End);
            }

            //Remove Deleted Dates
            foreach (var date in existingDates.Where(w => !request.Dates.Exists(a => a.Id == w.Id)))
            {
                Event.RemoveDate(date);
            }
        }
    }
}
