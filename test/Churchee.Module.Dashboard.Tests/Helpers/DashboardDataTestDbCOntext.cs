using Churchee.Common.Abstractions.Entities;
using Churchee.Data.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Dashboard.Tests.Helpers
{
    internal class DashboardDataTestDbContext : DbContext
    {
        public DashboardDataTestDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var entityRegistrations = new Dashboard.Registrations.EntityRegistrations();

            entityRegistrations.RegisterEntities(builder);

            var tenantId = Ids.TenantId;

            builder.ApplyGlobalFilters<ITenantedEntity>(a => a.ApplicationTenantId == tenantId);

            builder.ApplyGlobalFilters<IEntity>(a => !a.Deleted);
        }

    }
}
