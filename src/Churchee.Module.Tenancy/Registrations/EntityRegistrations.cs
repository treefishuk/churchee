using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Tenancy.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Tenancy.Data.Registrations
{
    public class EntityRegistrations : IEntityRegistration, IFrontEndEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationTenant>(etb =>
            {
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