using Ardalis.Specification;
using Churchee.Module.Events.Entities;

namespace Churchee.Module.Events.Specifications
{
    internal class EventByIdIncludingDatesSpecification : Specification<Event>
    {
        public EventByIdIncludingDatesSpecification(Guid eventId)
        {
            Query.Include(e => e.EventDates).Where(x => x.Id == eventId);
        }
    }
}
