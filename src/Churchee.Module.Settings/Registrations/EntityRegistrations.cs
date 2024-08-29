using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Settings.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Settings.Registration
{
    public class EntityRegistrations : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Setting>(etb =>
            {
                etb.ToTable("Settings");
                etb.HasKey(e => new { e.Id, e.ApplicationTenantId });
                etb.Property(e => e.Id);
            });

        }
    }
}
