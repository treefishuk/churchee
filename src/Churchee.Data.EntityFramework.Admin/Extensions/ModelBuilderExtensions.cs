using Churchee.Common.Attributes;
using Churchee.Data.EntityFramework.Admin.Converters;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Churchee.Data.EntityFramework.Admin.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void EncryptProtectedProperties(this ModelBuilder modelBuilder, string key)
        {
            var converter = new EncryptionConvertor(key);

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(string)))
            {
                object[] attributes = property.PropertyInfo?.GetCustomAttributes(typeof(EncryptPropertyAttribute), false);

                if (attributes != null && attributes.Length != 0)
                {
                    property.SetValueConverter(converter);
                }
            }
        }

    }
}
