using Ardalis.Specification;
using Churchee.Module.Events.Entities;

namespace Churchee.Module.ChurchSuite.Events.Specifications
{
    public class GetEventByChurchSuiteSequenceSpecification : Specification<Event>
    {
        public GetEventByChurchSuiteSequenceSpecification(string sequence)
        {
            Query.IgnoreQueryFilters();

            Query.Where(x => x.SourceId == sequence);

            Query.Include(i => i.EventDates);
        }

    }
}
