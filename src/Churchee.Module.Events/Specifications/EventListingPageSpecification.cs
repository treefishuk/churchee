using Ardalis.Specification;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;

namespace Churchee.Module.Events.Specifications
{
    public class EventListingPageSpecification : Specification<Page>
    {
        public EventListingPageSpecification(Guid applicationTenantId)
        {
            Query.IgnoreQueryFilters().Where(e => e.PageType.SystemKey == PageTypes.EventListingPageTypeId && e.ApplicationTenantId == applicationTenantId);
        }

        public EventListingPageSpecification()
        {
            Query.Where(e => e.PageType.SystemKey == PageTypes.EventListingPageTypeId);
        }
    }
}
