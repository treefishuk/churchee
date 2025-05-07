using Ardalis.Specification;
using Churchee.Module.Videos.Entities;

namespace Churchee.Module.Videos.Specifications
{
    internal class VideoSearchFilterSpecification : Specification<Video>
    {
        public VideoSearchFilterSpecification(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return;
            }

            Query.Where(w => w.Title.Contains(searchText));
        }

    }
}
