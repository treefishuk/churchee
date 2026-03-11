using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Reviews.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Reviews.Registrations
{
    public class EntityRegistrations : IEntityRegistration, IFrontEndEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>(etb =>
            {
                etb.ToTable("Reviews");
                etb.Property(e => e.Id);
                etb.Property(e => e.Comment).HasColumnType("nvarchar(4000)");
            });

        }
    }
}
