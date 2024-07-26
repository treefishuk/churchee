using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Common.Storage
{
    public interface IDataStore
    {
        IRepository<T> GetRepository<T>() where T : class, IEntity;

        void SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
