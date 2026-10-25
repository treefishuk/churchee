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

            modelBuilder.Entity<AnalyticsDaily>(etb =>
            {
                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.Date })
                   .HasDatabaseName("IX_AnalyticsDaily_AppTenant_Date");

                etb.HasIndex(i => new
                {
                    i.ApplicationTenantId,
                    i.Date
                })
                .IsUnique()
                .HasDatabaseName("UX_AnalyticsDaily_Tenant_Date");
            });

            modelBuilder.Entity<AnalyticsDailyDevices>(etb =>
            {
                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.Date })
                   .HasDatabaseName("IX_AnalyticsDailyDevices_AppTenant_Date");

                etb.HasIndex(i => new
                {
                    i.ApplicationTenantId,
                    i.Date,
                    i.Device
                })
                .IsUnique()
                .HasDatabaseName("UX_AnalyticsDailyDevices_Tenant_Date_Device");
            });

            modelBuilder.Entity<AnalyticsDailyPages>(etb =>
            {
                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.Date })
                   .HasDatabaseName("IX_AnalyticsDailyPages_AppTenant_Date");

                etb.HasIndex(i => new
                {
                    i.ApplicationTenantId,
                    i.Date,
                    i.Url
                })
                .IsUnique()
                .HasDatabaseName("UX_AnalyticsDailyPages_Tenant_Date_Url");

            });

            modelBuilder.Entity<AnalyticsDailyReferrers>(etb =>
            {
                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.Date })
                   .HasDatabaseName("IX_AnalyticsDailyReferrers_AppTenant_Date");

                etb.HasIndex(i => new
                {
                    i.ApplicationTenantId,
                    i.Date,
                    i.Referrer
                })
                .IsUnique()
                .HasDatabaseName("UX_AnalyticsDailyReferrers_Tenant_Date_Referrer");
            });

            modelBuilder.Entity<AnalyticsHourly>(etb =>
            {
                etb.HasIndex(i => new { i.ApplicationTenantId, i.Deleted, i.Date })
                   .HasDatabaseName("IX_AnalyticsHourly_AppTenant_Date");

                etb.HasIndex(i => new
                {
                    i.ApplicationTenantId,
                    i.Date,
                    i.HourOfDay
                })
                .IsUnique()
                .HasDatabaseName("UX_AnalyticsHourly_Tenant_Date_HourOfDay");
            });
        }

    }

}
