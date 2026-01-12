using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Churchee.Data.EntityFramework.Admin.Extensions;
using Churchee.Data.EntityFramework.Shared.Extensions;
using Churchee.Module.Identity.Entities;
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

namespace Churchee.Data.EntityFramework.Admin
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
        private static int _isInitialized = 0;

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        public ApplicationDbContext(ILogger<ApplicationDbContext> logger, DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider, IMediator mediator, IConfiguration configuration, ITenantResolver tenantResolver = null)
            : base(options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _mediator = mediator;
            _tenantResolver = tenantResolver;
            _configuration = configuration;

            // Atomically set _isInitialized to 1 and run initialization only for the first caller.
            if (Interlocked.Exchange(ref _isInitialized, 1) == 0)
            {
                // Migrate database (requires instance members, so must run here)
                Database.EnsureCreated();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var entityRegistrations = _serviceProvider.GetServices<IEntityRegistration>();

            foreach (var reg in entityRegistrations)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Entity Registration: {Entity}", reg.GetType().FullName);
                }

                reg.RegisterEntities(builder);
            }

            if (_tenantResolver != null)
            {
                builder.ApplyGlobalFilters<ITenantedEntity>(a => a.ApplicationTenantId == _tenantResolver.GetTenantId());
            }

            builder.ApplyGlobalFilters<IEntity>(a => !a.Deleted);

            builder.SetDefaultStringLengths(256);

            builder.EncryptProtectedProperties(_configuration.GetRequiredSection("Security")["EncryptionKey"]);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.Entries().ApplyTrimOnStringFields();

            int changeCount = await base.SaveChangesAsync(cancellationToken);

            await _mediator.DispatchDomainEventsAsync(this);

            return changeCount;
        }
    }
}
