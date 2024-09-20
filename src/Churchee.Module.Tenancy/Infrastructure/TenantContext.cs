using Churchee.Module.Tenancy.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Sites.db
{
    public class TenantContext : DbContext
    {

        public TenantContext(DbContextOptions<TenantContext> options)
               : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationTenant>(etb =>
            {
                etb.ToTable("ApplicationTenant");
                etb.HasKey(e => e.Id);
                etb.HasMany(e => e.Hosts)
                   .WithOne(o => o.ApplicationTenant)
                   .HasForeignKey(e => e.ApplicationTenantId);
            });

            modelBuilder.Entity<ApplicationFeature>(etb =>
            {
                etb.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ApplicationHost>(etb =>
            {
                etb.HasKey(e => e.Id);
            });
        }
    }
}
