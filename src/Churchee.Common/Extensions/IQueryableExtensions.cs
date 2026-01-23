using System.Linq.Dynamic.Core;

namespace System.Linq
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TResult> OrderBy<TResult>(this IQueryable<TResult> query, string propertyName, string direction)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return query;
            }

            string orderbyCombined = $"{propertyName} {direction}";

            return query.OrderBy(orderbyCombined);
        }
    }
}
