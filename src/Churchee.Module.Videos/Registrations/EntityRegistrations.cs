using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Videos.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Videos.Registrations
{
    public class EntityRegistrations : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Video>(etb =>
            {
                etb.ToTable("Videos");
                etb.Property(e => e.Id);

                etb.HasIndex(p => p.VideoUri).IsUnique();

            });

        }
    }
}
