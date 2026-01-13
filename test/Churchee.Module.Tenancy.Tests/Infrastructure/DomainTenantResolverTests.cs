using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Infrastructure;
using Churchee.Sites.db;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Churchee.Module.Tenancy.Tests.Infrastructure
{
    public class DomainTenantResolverTests
    {
        [Fact]
        public void GetTenantId_ReturnsEmpty_When_HttpContextIsNull()
        {
            // Arrange
            var context = GetContext();
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var configMock = new Mock<IConfiguration>();

            var resolver = new DomainTenantResolver(context, accessorMock.Object, memoryCache, configMock.Object);

            // Act
            var result = resolver.GetTenantId();

            // Assert
            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetTenantId_ReturnsCachedValue_When_MemoryHasEntry()
        {
            // Arrange
            string domain = "example.com";
            var expected = Guid.NewGuid();
            var context = GetContext();

            var accessorMock = new Mock<IHttpContextAccessor>();
            var ctx = new DefaultHttpContext();
            ctx.Request.Host = new HostString(domain);
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            memoryCache.Set($"{domain}_tenantId", expected);

            var configMock = new Mock<IConfiguration>();

            var resolver = new DomainTenantResolver(context, accessorMock.Object, memoryCache, configMock.Object);

            // Act
            var result = resolver.GetTenantId();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetTenantId_ReturnsValueFromDb_When_NotInCache()
        {
            // Arrange
            string domain = "dbhost.local";
            var expected = Guid.NewGuid();
            var context = GetContext();

            var hostEntity = new ApplicationHost(domain, expected);

            context.Set<ApplicationHost>().Add(hostEntity);
            context.SaveChanges();

            var accessorMock = new Mock<IHttpContextAccessor>();
            var ctx = new DefaultHttpContext();
            ctx.Request.Host = new HostString(domain);
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var configMock = new Mock<IConfiguration>();

            var resolver = new DomainTenantResolver(context, accessorMock.Object, memoryCache, configMock.Object);

            // Act
            var result = resolver.GetTenantId();

            // Assert
            result.Should().Be(expected);

            // Also ensure value was cached
            bool cachedExists = memoryCache.TryGetValue($"{domain}_tenantId", out Guid cached);
            cachedExists.Should().BeTrue();
            cached.Should().Be(expected);
        }

        [Fact]
        public void GetTenantDevName_ReturnsEmpty_When_HttpContextIsNull()
        {
            // Arrange
            var context = GetContext();
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var configMock = new Mock<IConfiguration>();

            var resolver = new DomainTenantResolver(context, accessorMock.Object, memoryCache, configMock.Object);

            // Act
            string result = resolver.GetTenantDevName();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetTenantDevName_ReturnsCachedValue_When_MemoryHasEntry()
        {
            // Arrange
            string domain = "tenant.example";
            string expectedName = "mytenant";
            var context = GetContext();
            var accessorMock = new Mock<IHttpContextAccessor>();
            var ctx = new DefaultHttpContext();
            ctx.Request.Host = new HostString(domain);
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            memoryCache.Set($"{domain}_tenantName", expectedName);

            var configMock = new Mock<IConfiguration>();

            var resolver = new DomainTenantResolver(context, accessorMock.Object, memoryCache, configMock.Object);

            // Act
            string result = resolver.GetTenantDevName();

            // Assert
            result.Should().Be(expectedName);
        }

        private static TenantContext GetContext()
        {
            var options = new DbContextOptionsBuilder<TenantContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new TenantContext(options);
            return context;
        }

        [Fact]
        public void GetCDNPrefix_ReplacesAsteriskWithTenantDevName()
        {
            // Arrange
            string domain = "cdn.example";
            string tenantName = "replaceme";
            string prefixTemplate = "https://cdn.example.com/*/assets";

            var inMemorySettings = new Dictionary<string, string?>
            {
                 { "Images:Prefix", prefixTemplate }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var context = GetContext();

            var accessorMock = new Mock<IHttpContextAccessor>();
            var ctx = new DefaultHttpContext();
            ctx.Request.Host = new HostString(domain);
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            // Pre-populate tenant name cache so GetTenantDevName returns a value without hitting DB
            memoryCache.Set($"{domain}_tenantName", tenantName);

            var resolver = new DomainTenantResolver(context, accessorMock.Object, memoryCache, configuration);

            // Act
            string result = resolver.GetCDNPrefix();

            // Assert
            result.Should().Be("https://cdn.example.com/replaceme/assets");
        }
    }
}