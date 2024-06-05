﻿using System.Reflection.Emit;
using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Site.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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

            modelbuilder.Entity<WebContentTypeProperty>(etb =>
            {
                etb.HasKey(e => e.Id);
                etb.Property(e => e.Id);
            });

            modelbuilder.Entity<WebContentTypeTypeMapping>(etb =>
            {
                etb.HasKey(t => new { t.ParentWebContentTypeId, t.ChildWebContentTypeId });

                etb.HasOne(pt => pt.ParentWebContentType)
                .WithMany(p => p.ChildrenTypes)
                .HasForeignKey(pt => pt.ParentWebContentTypeId)
                .OnDelete(DeleteBehavior.NoAction);

                etb.HasOne(pt => pt.ChildWebContentType)
                .WithMany(p => p.ParentTypes)
                .HasForeignKey(pt => pt.ChildWebContentTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            });

            modelbuilder.Entity<WebContentTypeContent>(etb =>
            {
                etb.HasKey(t => t.Id);
                etb.Property(t => t.Id).ValueGeneratedNever();
            });


            modelbuilder.Entity<PageContent>(etb =>
            {
                etb.ToTable("PageContent", b => b.IsTemporal());
                etb.HasKey(t => new { t.Id, t.PageTypeContentId });
                etb.Property(t => t.Value).HasColumnType("nvarchar(max)");
                etb.HasOne(pt => pt.Page)
                .WithMany(p => p.PageContent)
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
