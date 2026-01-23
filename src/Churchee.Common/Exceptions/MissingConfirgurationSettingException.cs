using System;

namespace Churchee.Common.Exceptions
{
    public class MissingConfigurationSettingException : Exception
    {
        public MissingConfigurationSettingException() : base("A configuration setting is missing")
        {
        }

        public MissingConfigurationSettingException(string message) : base(message)
        {
        }

        public MissingConfigurationSettingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
