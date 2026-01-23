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

namespace Churchee.Data.EntityFramework.Admin
{
    public class EFStorage : IDataStore
    {
        private readonly DbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string CreatedDate = "CreatedDate";
        private const string CreatedByUser = "CreatedByUser";
        private const string CreatedById = "CreatedById";
        private const string ModifiedDate = "ModifiedDate";
        private const string ModifiedByName = "ModifiedByName";
        private const string ModifiedById = "ModifiedById";

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

            var (userId, name) = GetUserInfo();

            switch (entry.State)
            {
                case EntityState.Modified:
                    SetModifiedFields(entry, name, userId);
                    break;
                case EntityState.Added:
                    SetCreatedFields(entry, name, userId);
                    break;
                default:
                    return;
            }
        }

        private (Guid UserId, string Name) GetUserInfo()
        {
            var userId = Guid.Empty;

            string name = "System";

            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var user = _httpContextAccessor.HttpContext.User;

                var nameClaim = user.Claims.FirstOrDefault(w => w.Type == ClaimTypes.Name);

                var idClaim = user.Claims.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier);

                name = nameClaim?.Value ?? "Unknown";
                userId = idClaim != null ? Guid.Parse(idClaim.Value) : Guid.Empty;
            }

            return (userId, name);
        }

        private static void SetCreatedFields(EntityEntry entry, string name, Guid userId)
        {
            if (entry.Property(CreatedDate).CurrentValue == null)
            {
                entry.Property(CreatedDate).CurrentValue = DateTime.Now;
            }
            entry.Property(CreatedByUser).CurrentValue = name;
            entry.Property(CreatedById).CurrentValue = userId;
        }

        private static void SetModifiedFields(EntityEntry entry, string name, Guid userId)
        {
            entry.Property(ModifiedDate).CurrentValue = DateTime.Now;
            entry.Property(ModifiedByName).CurrentValue = name;
            entry.Property(ModifiedById).CurrentValue = userId;
        }
    }
}
