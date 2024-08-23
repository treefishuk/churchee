using Churchee.Common.Attributes;
using Churchee.Data.EntityFramework.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Churchee.Data.EntityFramework.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyGlobalFilters<TInterface>(this ModelBuilder modelBuilder,
               Expression<Func<TInterface, bool>> expression)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.GetRootType() == entityType)
                {
                    if (entityType.ClrType.GetInterface(typeof(TInterface).Name) != null)
                    {
                        var parameterType = Expression.Parameter(entityType.ClrType);

                        var expressionFilter = ReplacingExpressionVisitor.
                            Replace(expression.Parameters.Single(), parameterType, expression.Body);

                        var builder = modelBuilder.Entity(entityType.ClrType);

                        if (builder.Metadata.GetQueryFilter() != null)
                        {
                            var currentQueryFilter = builder.Metadata.GetQueryFilter();

                            var currentExpressionFilter = ReplacingExpressionVisitor.Replace(
                                currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);

                            expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
                        }

                        modelBuilder.Entity(entityType.ClrType).
                            HasQueryFilter(Expression.Lambda(expressionFilter, parameterType));
                    }
                }
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
