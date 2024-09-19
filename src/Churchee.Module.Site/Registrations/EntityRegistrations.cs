﻿using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Site.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Registration
{
    public class EntityRegistrations : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<WebContent>(etb =>
            {
                etb.HasKey(e => e.Id);
                etb.Property(e => e.Id);

                etb.Property(t => t.PublishedData).HasColumnType("nvarchar(max)");

                etb.Property(t => t.Order).HasDefaultValue(10);

                etb.HasMany(m => m.RedirectUrls)
                .WithOne()
                .OnDelete(DeleteBehavior.NoAction);

                etb.HasOne(d => d.PageType)
                .WithMany(p => p.Pages)
                .HasForeignKey(d => d.PageTypeId)
                .HasPrincipalKey(x => x.Id);

                etb.ToTable("WebContent");
            });



            modelbuilder.Entity<Page>(etb =>
            {
                etb.ToTable("Pages");

                etb.Property<int?>("OldOrder").HasColumnName("Order");

            });

            modelbuilder.Entity<MediaItem>(etb =>
            {
                etb.ToTable("MediaItems");
                etb.Property(e => e.Id);
            });

            modelbuilder.Entity<MediaFolder>(etb =>
            {
                etb.ToTable("MediaFolders");
                etb.Property(e => e.Id);
                etb.HasMany(m => m.Children).WithOne(o => o.Parent).HasForeignKey(f => f.ParentId).OnDelete(DeleteBehavior.NoAction);
            });


            modelbuilder.Entity<PageType>(etb =>
            {
                etb.HasKey(e => e.Id);
                etb.Property(e => e.Id);

                etb.HasMany(m => m.Pages)
                .WithOne(o => o.PageType)
                .HasForeignKey(f => f.PageTypeId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.NoAction);

            });

            modelbuilder.Entity<PageTypeProperty>(etb =>
            {
                etb.HasKey(e => e.Id);
                etb.Property(e => e.Id);
            });

            modelbuilder.Entity<PageTypeTypeMapping>(etb =>
            {
                etb.HasKey(t => new { t.ParentPageTypeId, t.ChildPageTypeId });

                etb.HasOne(pt => pt.ParentPageType)
                .WithMany(p => p.ChildrenTypes)
                .HasForeignKey(pt => pt.ParentPageTypeId)
                .OnDelete(DeleteBehavior.NoAction);

                etb.HasOne(pt => pt.ChildPageType)
                .WithMany(p => p.ParentTypes)
                .HasForeignKey(pt => pt.ChildPageTypeId)
                .OnDelete(DeleteBehavior.NoAction);

                etb.HasQueryFilter(f => !f.ParentPageType.Deleted);
                etb.HasQueryFilter(f => !f.ChildPageType.Deleted);

            });

            modelbuilder.Entity<PageTypeContent>(etb =>
            {
                etb.HasKey(t => t.Id);
                etb.Property(t => t.Id).ValueGeneratedNever();
            });


            modelbuilder.Entity<PageContent>(etb =>
            {
                etb.ToTable("PageContent", b => b.IsTemporal());
                etb.HasKey(t => new { t.PageId, t.PageTypeContentId });
                etb.Property(t => t.Value).HasColumnType("nvarchar(max)");
                etb.HasOne(pt => pt.Page)
                .WithMany(p => p.PageContent)
                .HasForeignKey(k => k.PageId)
                .OnDelete(DeleteBehavior.Cascade);

            });


            modelbuilder.Entity<ViewTemplate>(etb =>
            {
                etb.ToTable("ViewTemplates", b => b.IsTemporal());

                etb.Property(t => t.Content).HasColumnType("nvarchar(max)");

                etb.Property(t => t.TenantLocation).HasComputedColumnSql("'/' + convert(nvarchar(36), ApplicationTenantId) + [Location]");

                etb.HasIndex(i => i.TenantLocation);

                etb.HasKey(t => new { t.Id });

            });

            modelbuilder.Entity<RedirectUrl>(etb =>
            {
                etb.HasKey(t => new { t.Id });

            });

            modelbuilder.Entity<Css>(etb =>
            {
                etb.ToTable("CSS", b => b.IsTemporal());

                etb.HasKey(t => new { t.Id });

                etb.Property(t => t.Styles).HasColumnType("nvarchar(max)");
                etb.Property(t => t.MinifiedStyles).HasColumnType("nvarchar(max)");


            });

            modelbuilder.Entity<Article>(etb =>
            {
                etb.ToTable("Articles");

                etb.Property(t => t.Content).HasColumnType("nvarchar(max)");

            });
        }
    }
}
