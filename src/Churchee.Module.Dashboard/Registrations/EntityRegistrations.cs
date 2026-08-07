using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Dashboard.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Dashboard.Registrations
{
    public class EntityRegistrations : IEntityRegistration, IFrontEndEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PageView>(etb =>
            {
                // Primary key
                etb.HasKey(u => u.Id);

                etb.ToTable("PageViews");


                etb.Property(p => p.ViewedAtHour)
                   .HasComputedColumnSql("DATEPART(hour, [ViewedAt])", stored: true);

                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.ViewedAt })
                   .IncludeProperties(i => new { i.IpAddress })
                   .HasDatabaseName("IX_PageViews_ApplicationTenantId_Deleted_ViewedAt");

                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.ViewedAt, i.Device, i.UserAgent })
                   .IncludeProperties(i => new { i.IpAddress, i.Url, i.Referrer })
                   .HasDatabaseName("IX_PageViews_ApplicationTenantId_Deleted_ViewedAt_Device_UserAgent");

                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.IpAddress, i.ViewedAt })
                   .IncludeProperties(i => new { i.Device, i.UserAgent })
                   .HasDatabaseName("IX_PageViews_ApplicationTenantId_Deleted_IpAddress_ViewedAt");

                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.Url, i.ViewedAt })
                   .IncludeProperties(i => new { i.IpAddress, i.Referrer })
                   .HasDatabaseName("IX_PageViews_AppTenant_Url_ViewedAt");

                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.Referrer, i.ViewedAt })
                   .IncludeProperties(i => new { i.Url, i.IpAddress })
                   .HasDatabaseName("IX_PageViews_AppTenant_Referrer_ViewedAt");

                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.ViewedAtHour })
                   .IncludeProperties(i => new { i.ViewedAt })
                   .HasDatabaseName("IX_PageViews_AppTenant_ViewedHour");

            });
        }
    }

}
