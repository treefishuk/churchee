using System.Linq;

namespace Churchee.Common.Abstractions.Utilities
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);

        TDestination Map<TDestination>(object source);

        void Map(object source, object destination);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        IQueryable<TDestination> SelectTo<TDestination>(IQueryable source);
    }
}
