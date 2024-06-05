using System;

namespace Churchee.Common.Exceptions
{
    public class MissingConfirgurationSettingException : Exception
    {
        public MissingConfirgurationSettingException() : base()
        {
        }

        public MissingConfirgurationSettingException(string message) : base(message)
        {
        }

        public MissingConfirgurationSettingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
