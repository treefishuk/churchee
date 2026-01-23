namespace Churchee.Module.X.Exceptions
{
    public class XSyncException : Exception
    {
        public XSyncException()
        {
        }

        public XSyncException(string? message) : base(message)
        {
        }

        public XSyncException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
