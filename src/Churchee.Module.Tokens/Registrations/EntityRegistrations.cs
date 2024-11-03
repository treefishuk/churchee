using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Tokens.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Tokens.Registrations
{
    public class EntityRegistrations : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Token>(etb =>
            {
                etb.HasKey(e => e.Id);
                etb.Property(e => e.Value).HasMaxLength(2000);
            });

        }
    }
}
