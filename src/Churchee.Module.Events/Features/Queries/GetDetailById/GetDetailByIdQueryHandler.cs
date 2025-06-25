using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Models;
using Churchee.Module.Events.Specifications;
using MediatR;

namespace Churchee.Module.Events.Features.Queries
{
    public class GetDetailByIdQueryHandler : IRequestHandler<GetDetailByIdQuery, GetDetailByIdResponse>
    {

        private readonly IDataStore _dataStore;

        public GetDetailByIdQueryHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<GetDetailByIdResponse> Handle(GetDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _dataStore.GetRepository<Event>().FirstOrDefaultAsync(new EventByIdIncludingDatesSpecification(request.Id), cancellationToken);

            var dateData = await _dataStore.GetRepository<EventDate>().GetListAsync(new EventDatesForEventSpecification(request.Id), s => new { s.Id, s.Start, s.End }, cancellationToken);

            var dateList = new List<EventDateModel>();

            if (dateData != null)
            {
                foreach (var date in dateData)
                {
                    dateList.Add(new EventDateModel(date.Id, date.Start, date.End));
                }
            }

            var response = new GetDetailByIdResponse.Builder()
                .SetTitle(entity.Title)
                .SetDescription(entity.Description)
                .SetContent(entity.Content)
                .SetImageUrl(entity.ImageUrl)
                .SetLocationName(entity.LocationName)
                .SetCity(entity.City)
                .SetStreet(entity.Street)
                .SetPostCode(entity.PostCode)
                .SetCountry(entity.Country)
                .SetLatitude(entity.Latitude)
                .SetLongitude(entity.Longitude)
                .SetDates(dateList)
                .Build();

            return response;
        }
    }
}
