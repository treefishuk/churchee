using System.Runtime.Serialization;

namespace Churchee.Module.Podcasts.Spotify.Exceptions
{
    public class PodcastSyncException : Exception
    {
        public PodcastSyncException()
        {
        }

        public PodcastSyncException(string? message) : base(message)
        {
        }

        public PodcastSyncException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PodcastSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
