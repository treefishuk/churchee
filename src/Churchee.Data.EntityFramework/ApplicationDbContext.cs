using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Churchee.Data.EntityFramework.Extensions;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Tenancy.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Data.EntityFramework
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IDataProtectionKeyContext
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMediator _mediator;
        private readonly ITenantResolver _tenantResolver;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Gets/sets whether the db context as been initialized. This
        /// is only performed once in the application lifecycle.
        /// </summary>
        private static volatile bool _isInitialized = false;

        /// <summary>
        /// The object mutex used for initializing the context.
        /// </summary>
        private static readonly object _mutex = new();

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        public ApplicationDbContext(ILogger<ApplicationDbContext> logger, DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider, IMediator mediator, ITenantResolver tenantResolver, IConfiguration configuration)
            : base(options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _mediator = mediator;
            _tenantResolver = tenantResolver;
            _configuration = configuration;

            if (!_isInitialized)
            {
                lock (_mutex)
                {
                    if (!_isInitialized)
                    {
                        // Migrate database
                        Database.EnsureCreated();

                        _isInitialized = true;
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var entityRegistrations = _serviceProvider.GetServices<IEntityRegistration>();

            foreach (var reg in entityRegistrations)
            {
                _logger.LogInformation("Entity Registration: {Entity}", reg.GetType().FullName);

                reg.RegisterEntities(builder);
            }

            builder.ApplyGlobalFilters<ITenantedEntity>(a => a.ApplicationTenantId == _tenantResolver.GetTenantId());

            builder.ApplyGlobalFilters<IEntity>(a => !a.Deleted);

            builder.SetDefaultStringLengths(256);

            builder.EncryptProtectedProperties(_configuration.GetRequiredSection("Security")["EncryptionKey"]);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.Entries().ApplyTrimOnStringFields();

            var changeCount = await base.SaveChangesAsync(cancellationToken);

            await _mediator.DispatchDomainEventsAsync(this);

            return changeCount;
        }
    }
}
