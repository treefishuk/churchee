using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Data.EntityFramework.Site
{
    public class SiteDataStore : IDataStore
    {
        private readonly DbContext _dbContext;

        public SiteDataStore(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<T> GetRepository<T>() where T : class, IEntity
        {
            return new ReadonlyGenericRepository<T>(_dbContext);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Not supported in the readonly version of this repository");
        }

        public void SaveChanges()
        {
            throw new InvalidOperationException("Not supported in the readonly version of this repository");
        }
    }
}
