using Churchee.Module.Logging.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

namespace Churchee.Module.Logging.Tests.Registrations
{
    public class ServiceRegistrationsTests
    {
        [Fact]
        public void ServiceRegistrations_Registers_LoggerProvider_And_LogsDbContext()
        {
            // Arrange
            var services = new ServiceCollection();

            // Use in-memory configuration for connection string
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"ConnectionStrings:LogsConnection", "Server=(localdb)\\mssqllocaldb;Database=LogsTestDb;Trusted_Connection=True;"}
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            services.AddSingleton(configuration);

            var serviceProvider = services.BuildServiceProvider();

            var registrations = new Churchee.Data.EntityFramework.ServiceRegistrations();

            // Act
            registrations.Execute(services, serviceProvider);
            serviceProvider = services.BuildServiceProvider();

            // Assert
            var loggerProvider = serviceProvider.GetService<ILoggerProvider>();
            Assert.NotNull(loggerProvider);
            Assert.IsType<SerilogLoggerProvider>(loggerProvider);

            var dbContext = serviceProvider.GetService<LogsDBContext>();
            Assert.NotNull(dbContext);
            Assert.IsAssignableFrom<DbContext>(dbContext);
        }

        [Fact]
        public void Priority_ShouldBe5000()
        {
            // Arrange
            var registrations = new Churchee.Data.EntityFramework.ServiceRegistrations();

            // Act & Assert
            Assert.Equal(5000, registrations.Priority);
        }
    }
}
