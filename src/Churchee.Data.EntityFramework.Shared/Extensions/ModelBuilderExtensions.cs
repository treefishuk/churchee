using Churchee.Common.Attributes;
using Churchee.Data.EntityFramework.Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Churchee.Data.EntityFramework.Shared.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyGlobalFilters<TInterface>(this ModelBuilder modelBuilder,
               Expression<Func<TInterface, bool>> expression)
        {
            foreach (var clrType in modelBuilder.Model.GetEntityTypes()
                .Where(entityType => entityType.GetRootType() == entityType && entityType.ClrType.GetInterface(typeof(TInterface).Name) != null)
                .Select(s => s.ClrType))
            {
                var parameterType = Expression.Parameter(clrType);

                var expressionFilter = ReplacingExpressionVisitor.
                    Replace(expression.Parameters.Single(), parameterType, expression.Body);

                var builder = modelBuilder.Entity(clrType);

                var declaredFilters = builder.Metadata.GetDeclaredQueryFilters();

                var currentQueryFilter = declaredFilters.FirstOrDefault()?.Expression;

                if (currentQueryFilter != null)
                {
                    var currentExpressionFilter = ReplacingExpressionVisitor.Replace(
                        currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);

                    expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
                }

                modelBuilder.Entity(clrType).
                    HasQueryFilter(Expression.Lambda(expressionFilter, parameterType));
            }
        }

        public static void SetDefaultStringLengths(this ModelBuilder modelBuilder, int length)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(string)))
            {

                string colType = property.GetColumnType() ?? string.Empty;

                string colTypeNormalized = colType.ToUpperInvariant();

                if (property.GetMaxLength() == null && colTypeNormalized != "NVARCHAR(MAX)")
                {
                    property.SetMaxLength(length);
                }
            }
        }

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
