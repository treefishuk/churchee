using Churchee.Common.Abstractions.Storage;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Data.EntityFramework.Admin.Registrations
{
    internal class DataProtectionKeyConfiguration : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataProtectionKey>(etb =>
            {
                etb.Property(t => t.Xml).HasMaxLength(4000);
            });
        }
    }
}
