using Churchee.Module.Logging.Infrastructure;
using Churchee.Module.Logging.Registrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Testcontainers.MsSql;

namespace Churchee.Module.Logging.Tests.Registrations
{
    public class ServiceRegistrationsTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;

        public ServiceRegistrationsTests()
        {
            _msSqlContainer = new MsSqlBuilder()
             .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
             .WithPassword("yourStrong(!)Password")
             .Build();
        }

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _msSqlContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public void ServiceRegistrations_Registers_LoggerProvider_And_LogsDbContext()
        {
            // Arrange
            var services = new ServiceCollection();

            // Use in-memory configuration for connection string
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"ConnectionStrings:LogsConnection", _msSqlContainer.GetConnectionString()}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            services.AddSingleton(configuration);

            var serviceProvider = services.BuildServiceProvider();

            var registrations = new ServiceRegistrations();

            // Act
            registrations.Execute(services, serviceProvider);
            serviceProvider = services.BuildServiceProvider();

            // Assert
            var loggerProvider = serviceProvider.GetService<ILoggerProvider>();
            Assert.NotNull(loggerProvider);
            Assert.IsType<SerilogLoggerProvider>(loggerProvider);

            var dbContext = serviceProvider.GetService<LogsDBContext>();
            Assert.NotNull(dbContext);
            Assert.IsType<DbContext>(dbContext, exactMatch: false);
        }

        [Fact]
        public void Priority_ShouldBe5000()
        {
            // Arrange
            var registrations = new ServiceRegistrations();

            // Act & Assert
            Assert.Equal(5000, registrations.Priority);
        }
    }
}
