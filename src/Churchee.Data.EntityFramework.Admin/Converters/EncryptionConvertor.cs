using Churchee.Common.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Churchee.Data.EntityFramework.Admin.Converters
{
    public class EncryptionConvertor : ValueConverter<string, string>
    {
        public EncryptionConvertor(string key, ConverterMappingHints mappingHints = null)
            : base(x => EncryptFunc(key, x),
                   x => DecryptFunc(key, x), mappingHints)
        {

        }

        private static string EncryptFunc(string key, string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return string.Empty;
            }

            return AesEncryptionHelper.Encrypt(key, val);
        }

        private static string DecryptFunc(string key, string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return string.Empty;
            }
            return AesEncryptionHelper.Decrypt(key, val);
        }
    }
}
