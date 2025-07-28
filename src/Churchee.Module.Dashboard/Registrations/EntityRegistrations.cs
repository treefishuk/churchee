using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Dashboard.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Dashboard.Registrations
{
    public class EntityRegistrations : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PageView>(etb =>
            {
                // Primary key
                etb.HasKey(u => u.Id);

                etb.ToTable("PageViews");

                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.ViewedAt })
                   .IncludeProperties(i => new { i.IpAddress });

                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.ViewedAt, i.Device, i.UserAgent })
                   .IncludeProperties(i => new { i.IpAddress, i.Url, i.Referrer });

            });
        }
    }

}
