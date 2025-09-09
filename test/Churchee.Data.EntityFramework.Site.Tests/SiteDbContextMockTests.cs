using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Tenancy.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Churchee.Data.EntityFramework.Site.Tests
{
    public class SiteDbContextMockTests
    {
        public class TestEntity : ITenantedEntity, IEntity
        {
            public int Id { get; set; }
            public Guid ApplicationTenantId { get; set; }
            public bool Deleted { get; set; }
        }

        public class PageView
        {
            public int Id { get; set; }
        }

        public class FilterEntity : ITenantedEntity, IEntity
        {
            public int Id { get; set; }
            public Guid ApplicationTenantId { get; set; }
            public bool Deleted { get; set; }
        }

        private static SiteDbContext CreateContext(
            Guid? tenantId = null,
            bool includeFilterEntity = false)
        {
            var options = new DbContextOptionsBuilder<SiteDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var tenantResolver = new Mock<ITenantResolver>();
            tenantResolver.Setup(t => t.GetTenantId()).Returns(tenantId ?? Guid.NewGuid());

            var logger = new Mock<ILogger<SiteDbContext>>();

            var services = new ServiceCollection();

            // Always register core entities; optionally register FilterEntity so it is present when model is first built
            services.AddSingleton<IFrontEndEntityRegistration>(new CoreRegistration());

            var serviceProvider = services.BuildServiceProvider();

            var ctx = new SiteDbContext(options, tenantResolver.Object, serviceProvider, logger.Object);

            // Force model build now so global filters apply to all needed entities
            _ = ctx.Model;

            return ctx;
        }

        private sealed class CoreRegistration : IFrontEndEntityRegistration
        {
            public void RegisterEntities(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<PageView>().HasKey(e => e.Id);
                modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
                modelBuilder.Entity<FilterEntity>().HasKey(e => e.Id);
            }
        }

        [Fact]
        public void OnModelCreating_Calls_EntityRegistrations()
        {
            using var ctx = CreateContext();
            Assert.NotNull(ctx.Model.FindEntityType(typeof(TestEntity)));
        }

        [Fact]
        public void SaveChanges_WhenAnyNonPageView_Throws()
        {
            using var ctx = CreateContext();
            ctx.Add(new PageView { Id = 1 });
            ctx.Add(new TestEntity { Id = 2, ApplicationTenantId = Guid.NewGuid(), Deleted = false });
            Assert.Throws<InvalidOperationException>(() => ctx.SaveChanges());
        }

        [Fact]
        public async Task SaveChangesAsync_WhenAnyNonPageView_Throws()
        {
            using var ctx = CreateContext();
            ctx.Add(new PageView { Id = 1 });
            ctx.Add(new TestEntity { Id = 2, ApplicationTenantId = Guid.NewGuid(), Deleted = false });
            await Assert.ThrowsAsync<InvalidOperationException>(() => ctx.SaveChangesAsync());
        }

        [Fact]
        public void SaveChanges_WhenAllPageView_DoesNotThrow()
        {
            var tenantId = Guid.NewGuid();
            using var ctx = CreateContext(tenantId);
            ctx.Add(new PageView { Id = 1 });
            int saved = ctx.SaveChanges();
            Assert.Equal(1, saved);
        }

        [Fact]
        public async Task SaveChangesAsync_WhenAllPageView_DoesNotThrow()
        {
            var tenantId = Guid.NewGuid();
            using var ctx = CreateContext(tenantId);
            ctx.Add(new PageView { Id = 1 });
            int saved = await ctx.SaveChangesAsync();
            Assert.Equal(1, saved);
        }
    }
}