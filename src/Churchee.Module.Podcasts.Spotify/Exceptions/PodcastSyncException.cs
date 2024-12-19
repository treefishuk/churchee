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
    }
}
