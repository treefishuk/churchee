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
        }

    }
}
