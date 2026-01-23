using Ardalis.Specification;
using Churchee.Module.Events.Entities;

namespace Churchee.Module.Facebook.Events.Specifications
{
    public class GetEventByFacebookIdSpecification : Specification<Event>
    {
        public GetEventByFacebookIdSpecification(string facebookEventId)
        {
            Query.IgnoreQueryFilters();

            Query.Where(x => x.SourceId == facebookEventId);
        }

    }
}
