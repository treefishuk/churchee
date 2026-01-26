using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Events.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Events.Registration
{
    public class EntityRegistrations : IEntityRegistration, IFrontEndEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(etb =>
            {
                etb.ToTable("Events");

                etb.Property(e => e.Latitude).HasPrecision(8, 6);

                etb.Property(e => e.Longitude).HasPrecision(9, 6);

                etb.Property(e => e.PostCode).HasMaxLength(20);

                etb.Property(t => t.Content).HasColumnType("nvarchar(max)");

                etb.HasMany(x => x.EventDates).WithOne(o => o.Event).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);

                /// Legacy Property - Not Used but kept for backwards compatibility
                etb.Property<string>("LegacyImageUrl").HasColumnName("ImageUrl");

            });

            modelBuilder.Entity<EventDate>(etb =>
            {
                etb.ToTable("EventDates");

                etb.Property(p => p.Id).ValueGeneratedNever();
            });

        }
    }
}
