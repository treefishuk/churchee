namespace Churchee.Module.YouTube.Exceptions
{
    public class YouTubeSyncException : Exception
    {
        public YouTubeSyncException()
        {
        }

        public YouTubeSyncException(string? message) : base(message)
        {
        }

        public YouTubeSyncException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
