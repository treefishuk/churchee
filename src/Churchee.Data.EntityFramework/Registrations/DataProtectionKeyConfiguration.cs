using Churchee.Common.Abstractions.Storage;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Data.EntityFramework.Registrations
{
    internal class DataProtectionKeyConfiguration : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<DataProtectionKey>(etb =>
            {
                etb.Property(t => t.Xml).HasMaxLength(4000);
            });
        }
    }
}
