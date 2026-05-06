namespace Churchee.Module.Google.Reviews.Exceptions
{
    public class GoogleReviewSyncException : Exception
    {
        public GoogleReviewSyncException()
        {
        }

        public GoogleReviewSyncException(string message) : base(message)
        {
        }

        public GoogleReviewSyncException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
