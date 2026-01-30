using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Churchee.Data.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Churchee.Data.EntityFramework.Site
{
    public class SiteDbContext : DbContext
    {
        private readonly ILogger _logger;
        private readonly ITenantResolver _tenantResolver;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public SiteDbContext(DbContextOptions<SiteDbContext> options, ITenantResolver tenantResolver, IServiceProvider serviceProvider, ILogger<SiteDbContext> logger, IConfiguration configuration)
               : base(options)
        {
            _tenantResolver = tenantResolver;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entityRegistrations = _serviceProvider.GetServices<IFrontEndEntityRegistration>();

            foreach (var reg in entityRegistrations)
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Site Entity Registration: {Entity}", reg.GetType().FullName);
                }

                reg.RegisterEntities(modelBuilder);
            }

            modelBuilder.ApplyGlobalFilters<ITenantedEntity>(a => a.ApplicationTenantId == _tenantResolver.GetTenantId());

            modelBuilder.ApplyGlobalFilters<IEntity>(a => !a.Deleted);

            modelBuilder.EncryptProtectedProperties(_configuration.GetRequiredSection("Security")["EncryptionKey"]);
        }

        public override int SaveChanges()
        {
            return ChangeTracker.Entries().Any(e => e.State == EntityState.Added && e.Entity.GetType().Name != "PageView")
                ? throw new InvalidOperationException("This context is read-only except for PageView entities.")
                : base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return ChangeTracker.Entries().Any(e => e.State == EntityState.Added && e.Entity.GetType().Name != "PageView")
                ? throw new InvalidOperationException("This context is read-only except for PageView entities.")
                : base.SaveChangesAsync(cancellationToken);
        }

    }
}
