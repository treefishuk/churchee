using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Events.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Events.Registration
{
    public class EntityRegistrations : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Event>(etb =>
            {
                etb.ToTable("Events");

                etb.Property(e => e.Latitude).HasPrecision(8,6);

                etb.Property(e => e.Longitude).HasPrecision(9,6);

                etb.Property(e => e.PostCode).HasMaxLength(20);

                etb.Property(t => t.Content).HasColumnType("nvarchar(max)");

            });

        }
    }
}
