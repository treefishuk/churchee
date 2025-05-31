using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Registrations;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Churchee.Module.Identity.Tests.Registrations
{
    public class EntityRegistrationIntegrationTests : IDisposable
    {
        private readonly MsSqlContainer _msSqlContainer;
        private readonly DbContextOptions<IdentityDbContext> _dbContextOptions;

        public EntityRegistrationIntegrationTests()
        {
            _msSqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("yourStrong(!)Password")
                .Build();

            // Await the StartAsync method properly to avoid directly accessing ValueTask result
            _msSqlContainer.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            string connectionString = _msSqlContainer.GetConnectionString();

            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            _dbContextOptions = optionsBuilder.Options;

            using var context = new IdentityDbContext(_dbContextOptions);
            context.Database.EnsureCreated();
        }

        [Fact]
        public void VerifyEntityConfigurations()
        {
            using var context = new IdentityDbContext(_dbContextOptions);
            var modelBuilder = new ModelBuilder();

            var entityRegistrations = new EntityRegistrations();
            entityRegistrations.RegisterEntities(modelBuilder);

            // Verify the configurations
            var userEntity = modelBuilder.Model.FindEntityType(typeof(ApplicationUser));
            Assert.NotNull(userEntity);
            Assert.Equal("AspNetUsers", userEntity.GetTableName());

            var roleEntity = modelBuilder.Model.FindEntityType(typeof(ApplicationRole));
            Assert.NotNull(roleEntity);
            Assert.Equal("AspNetRoles", roleEntity.GetTableName());

            // Add more assertions as needed
        }

        public void Dispose()
        {
            if (_msSqlContainer == null)
            {
                return;
            }

            _msSqlContainer.StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            _msSqlContainer.DisposeAsync().AsTask().GetAwaiter().GetResult();

            GC.SuppressFinalize(this);
        }
    }

    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        // Add other DbSet properties as needed

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityRegistrations = new EntityRegistrations();
            entityRegistrations.RegisterEntities(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
