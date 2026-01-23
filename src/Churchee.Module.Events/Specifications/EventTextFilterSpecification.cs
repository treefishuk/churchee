using Ardalis.Specification;
using Churchee.Module.Events.Entities;

namespace Churchee.Module.Events.Specifications
{
    public class EventTextFilterSpecification : Specification<Event>
    {
        public EventTextFilterSpecification(string text)
        {
            Query.Where(x => x.Title.Contains(text));
        }
    }
}
