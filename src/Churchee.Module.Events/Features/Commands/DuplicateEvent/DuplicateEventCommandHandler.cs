using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using MediatR;

namespace Churchee.Module.Events.Features.Commands.DuplicateEvent
{
    public class DuplicateEventCommandHandler : IRequestHandler<DuplicateEventCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        public DuplicateEventCommandHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(DuplicateEventCommand request, CancellationToken cancellationToken)
        {
            var tenantId = await _currentUser.GetApplicationTenantId();

            var repo = _dataStore.GetRepository<Event>();

            var originalEvent = await repo.FirstOrDefaultAsync(new EventByIdIncludingDatesSpecification(request.OriginalEventId), s => new
            {
                s.Title,
                s.LocationName,
                s.Street,
                s.City,
                s.PostCode,
                s.Longitude,
                s.Latitude,
                s.Content,
                s.Description,
                s.Country,
                s.PageTypeId,
                s.ParentId,
                ParentSlug = s.Parent.Url,
                s.ImageUrl
            }, cancellationToken);

            var newEvent = new Event.Builder()
                .SetTitle(originalEvent.Title + " - Copy")
                .SetSourceName("Churchee")
                .SetLocationName(originalEvent.LocationName)
                .SetStreet(originalEvent.Street)
                .SetCity(originalEvent.City)
                .SetPostCode(originalEvent.PostCode)
                .SetLongitude(originalEvent.Longitude)
                .SetLatitude(originalEvent.Latitude)
                .SetContent(originalEvent.Content)
                .SetDescription(originalEvent.Description)
                .SetCountry(originalEvent.Country)
                .SetPageTypeId(originalEvent.PageTypeId.Value)
                .SetParentId(originalEvent.ParentId)
                .SetParentSlug(originalEvent.ParentSlug)
                .SetApplicationTenantId(tenantId)
                .SetImageUrl(originalEvent.ImageUrl)
                .Build();

            repo.Create(newEvent);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
