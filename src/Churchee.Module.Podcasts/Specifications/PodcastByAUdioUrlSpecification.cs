using Ardalis.Specification;
using Churchee.Module.Podcasts.Entities;

namespace Churchee.Module.Podcasts.Specifications
{
    public class PodcastByAudioUrlSpecification : Specification<Podcast>
    {
        public PodcastByAudioUrlSpecification(string url)
        {
            Query.Where(w => w.AudioUri == url);
        }
    }
}
