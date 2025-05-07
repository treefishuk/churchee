using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Data.EntityFramework
{
    public class EFStorage : IDataStore
    {
        private readonly DbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EFStorage(DbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public IRepository<T> GetRepository<T>() where T : class, IEntity
        {
            return new GenericRepository<T>(_dbContext);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = _dbContext.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                AssignAutoValues(entry);
            }

            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        private void AssignAutoValues(EntityEntry entry)
        {
            if (!entry.Entity.ImplementsInterface<ITrackable>())
            {
                return;
            }

            if (_httpContextAccessor.HttpContext != null)
            {
                var user = _httpContextAccessor.HttpContext.User;

                if (user != null && user.Identity.IsAuthenticated)
                {
                    var nameClaim = user.Claims.FirstOrDefault(w => w.Type == ClaimTypes.Name);
                    var idClaim = user.Claims.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier);

                    string name = nameClaim?.Value ?? "Unknown";
                    var userId = idClaim != null ? Guid.Parse(idClaim.Value) : Guid.Empty;

                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entry.Property("ModifiedDate").CurrentValue = DateTime.Now;
                            entry.Property("ModifiedByName").CurrentValue = name;
                            entry.Property("ModifiedById").CurrentValue = userId;
                            break;
                        case EntityState.Added:
                            if (entry.Property("CreatedDate").CurrentValue == null)
                            {
                                entry.Property("CreatedDate").CurrentValue = DateTime.Now;
                            }
                            entry.Property("CreatedByUser").CurrentValue = name;
                            entry.Property("CreatedById").CurrentValue = userId;
                            break;
                        default:
                            return;
                    }
                }
            }
            else
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Property("ModifiedDate").CurrentValue = DateTime.Now;
                        entry.Property("ModifiedByName").CurrentValue = "System";
                        entry.Property("ModifiedById").CurrentValue = Guid.Empty;
                        break;
                    case EntityState.Added:
                        if (entry.Property("CreatedDate").CurrentValue == null)
                        {
                            entry.Property("CreatedDate").CurrentValue = DateTime.Now;
                        }
                        entry.Property("CreatedByUser").CurrentValue = "System";
                        entry.Property("CreatedById").CurrentValue = Guid.Empty;
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
