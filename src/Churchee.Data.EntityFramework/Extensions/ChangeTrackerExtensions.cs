using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;

namespace Churchee.Data.EntityFramework.Extensions
{
    internal static class ChangeTrackerExtensions
    {


        internal static IEnumerable<EntityEntry> ApplyTrimOnStringFields(this IEnumerable<EntityEntry> entities)
        {
            foreach (var entity in entities)
            {
                foreach (PropertyEntry property in entity.Properties.Where(o => o.Metadata.ClrType.Name.Equals("String") && o.CurrentValue is not null))
                {
                    TrimFieldValue(property);
                }
            }

            return entities;

        }

        private static void TrimFieldValue(PropertyEntry property)
        {
            if (property.CurrentValue == null)
            {
                return;
            }

            var currentValue = property.CurrentValue.ToString();

            var metaData = property.Metadata;

            var maxLength = metaData.GetMaxLength();

            if (!maxLength.HasValue)
            {
                property.CurrentValue = currentValue.Trim();

                return;
            }

            if (currentValue.Length > maxLength.Value)
            {
                property.CurrentValue = currentValue.Substring(0, maxLength.Value).Trim();
            }
        }


    }
}
