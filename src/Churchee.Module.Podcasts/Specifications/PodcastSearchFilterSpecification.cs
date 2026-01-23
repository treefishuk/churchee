using Ardalis.Specification;
using Churchee.Module.Podcasts.Entities;

namespace Churchee.Module.Podcasts.Specifications
{
    internal class PodcastSearchFilterSpecification : Specification<Podcast>
    {
        public PodcastSearchFilterSpecification(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return;
            }

            Query.Where(w => w.Title.Contains(searchText));
        }

    }
}
