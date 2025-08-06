using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Churchee.Data.EntityFramework.Shared.Extensions;
using Churchee.Module.Tenancy.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Churchee.Data.EntityFramework.Site
{
    public class SiteDbContext : DbContext
    {
        private readonly ILogger _logger;
        private readonly ITenantResolver _tenantResolver;
        private readonly IServiceProvider _serviceProvider;

        public SiteDbContext(DbContextOptions<SiteDbContext> options, ITenantResolver tenantResolver, IServiceProvider serviceProvider, ILogger<SiteDbContext> logger)
               : base(options)
        {
            _tenantResolver = tenantResolver;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var entityRegistrations = _serviceProvider.GetServices<IFrontEndEntityRegistration>();

            foreach (var reg in entityRegistrations)
            {
                _logger.LogInformation("Site Entity Registration: {Entity}", reg.GetType().FullName);

                reg.RegisterEntities(builder);
            }

            builder.ApplyGlobalFilters<ITenantedEntity>(a => a.ApplicationTenantId == _tenantResolver.GetTenantId());

            builder.ApplyGlobalFilters<IEntity>(a => a.Deleted == false);

        }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("This context is read-only.");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("This context is read-only.");
        }

    }
}
