using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Specifications
{
    public class MediaFolderByIdSpecification : Specification<MediaFolder>
    {
        public MediaFolderByIdSpecification(Guid folderId)
        {
            Query.Where(x => x.Id == folderId);
        }

    }
}
