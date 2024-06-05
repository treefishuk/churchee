using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Tenancy.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Tenancy.Data.Registrations
{
    public class EntityRegistrations : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<ApplicationTenant>(etb =>
            {
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