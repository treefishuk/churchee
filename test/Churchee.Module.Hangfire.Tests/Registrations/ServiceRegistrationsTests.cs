using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Hangfire.Registrations;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Churchee.Module.Hangfire.Tests.Registrations
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
        public void ServicesRegisterations_SetsPriority()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var cut = new ServiceRegistrations();

            // Assert
            cut.Priority.Should().Be(200);
        }

        [Fact]
        public void ServicesRegisterations_Execute_Registers_Services()
        {
            // Arrange
            var services = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string?> {
                {"ConnectionStrings:HangfireConnection", _msSqlContainer.GetConnectionString()}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            services.AddSingleton(configuration);

            var cut = new ServiceRegistrations();

            // Act
            cut.Execute(services, services.BuildServiceProvider());

            var serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetService<IJobService>().Should().NotBeNull();
        }
    }
}
