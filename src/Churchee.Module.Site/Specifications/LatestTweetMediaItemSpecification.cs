using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class LatestTweetMediaItemSpecification : Specification<MediaItem>
    {
        public LatestTweetMediaItemSpecification(Guid applicationTenantId)
        {
            Query.IgnoreQueryFilters();
            Query.Where(x => x.ApplicationTenantId == applicationTenantId && x.Title.StartsWith("Tweet: "))
                 .OrderByDescending(x => x.CreatedDate)
                 .Take(1);
        }
    }
}
