using System.Runtime.Serialization;

namespace Churchee.Infrastructure.SassCompilation.Exceptions
{
    public class SassCompilationException : Exception
    {
        public SassCompilationException()
        {
        }

        public SassCompilationException(string? message) : base(message)
        {
        }

        public SassCompilationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SassCompilationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
