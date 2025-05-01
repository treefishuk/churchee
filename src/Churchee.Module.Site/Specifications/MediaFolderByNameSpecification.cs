using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class MediaFolderByNameSpecification : Specification<MediaFolder>
    {
        public MediaFolderByNameSpecification(string folderName, Guid applicationTenantId)
        {
            Query.IgnoreQueryFilters();

            Query.Where(x => x.Name == folderName && x.ApplicationTenantId == applicationTenantId);
        }

    }
}
