using Ardalis.Specification;
using Churchee.Module.Events.Entities;

namespace Churchee.Module.ChurchSuite.Events.Specifications
{
    public class GetEventByChurchSuiteSequenceSpecification : Specification<Event>
    {
        public GetEventByChurchSuiteSequenceSpecification(string sequence, Guid applicationTenantId)
        {
            Query.IgnoreQueryFilters();

            Query.Where(x => x.SourceId == sequence && x.ApplicationTenantId == applicationTenantId);

            Query.Include(i => i.EventDates);
        }

    }
}
