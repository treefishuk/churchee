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

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<ApplicationTenant>(etb =>
            {
                etb.ToTable("ApplicationTenant");
                etb.HasKey(e => e.Id);
                etb.HasMany(e => e.Hosts)
                   .WithOne(o => o.ApplicationTenant)
                   .HasForeignKey(e => e.ApplicationTenantId);
            });

            modelbuilder.Entity<ApplicationFeature>(etb =>
            {
                etb.HasKey(e => e.Id);
            });

            modelbuilder.Entity<ApplicationHost>(etb =>
            {
                etb.HasKey(e => e.Id);
            });
        }
    }
}
