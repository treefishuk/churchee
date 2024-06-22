using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Models;
using Churchee.Module.Events.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var entity = await _dataStore.GetRepository<Event>().ApplySpecification(new EventByIdIncludingDatesSpecification(request.Id)).FirstOrDefaultAsync();

            var dateData = await _dataStore.GetRepository<EventDate>().ApplySpecification(new EventDatesForEventSpecification(request.Id)).Select(s => new { s.Id, s.Start, s.End }).ToListAsync();

            var dateList = new List<EventDateModel>();

            if (dateData != null)
            {
                foreach (var date in dateData)
                {
                    dateList.Add(new EventDateModel(date.Id, date.Start, date.End));
                }
            }

            var response = new GetDetailByIdResponse(
                title: entity.Title,
                description: entity.Description,
                content: entity.Content,
                imageUrl: entity.ImageUrl,
                locationName: entity.LocationName,
                city: entity.City,
                street: entity.Street,
                postCode: entity.PostCode,
                country: entity.Country,
                latitude: entity.Latitude,
                longitude: entity.Longitude,
                dates: dateList);

            return response;
        }
    }
}
