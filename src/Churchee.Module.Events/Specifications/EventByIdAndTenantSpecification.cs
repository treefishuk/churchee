using Ardalis.Specification;
using Churchee.Module.Events.Entities;

namespace Churchee.Module.Events.Specifications
{
    public class EventByIdAndTenantSpecification : Specification<Event>
    {
        public EventByIdAndTenantSpecification(Guid applicationTenantId, Guid eventId)
        {
            Query
                .IgnoreQueryFilters()
                .Where(x => x.ApplicationTenantId == applicationTenantId && x.Id == eventId);
        }
    }
}