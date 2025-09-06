using Churchee.Module.Hangfire.Registrations;
using Churchee.Test.Helpers.Validation;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
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
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_PID", "Express")
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
        public void ServiceRegistrations_WithIsServiceSetToTrue_RegistersHanginterfacesOnly()
        {
            // Arrange
            var services = new ServiceCollection();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(s => s.GetSection("Hangfire")["IsService"]).Returns("true");
            configurationMock.Setup(s => s.GetSection("ConnectionStrings")["HangfireConnection"]).Returns(GetHangfireConnectionString());

            services.AddSingleton(configurationMock.Object);

            var cut = new ServiceRegistrations();

            var tempProvider = services.BuildServiceProvider(); // Do NOT dispose this

            // Act
            cut.Execute(services, tempProvider);

            using var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<IRecurringJobManager>().Should().NotBeNull();
            serviceProvider.GetService<IBackgroundJobClient>().Should().NotBeNull();
            serviceProvider.GetService<IHostedService>().Should().BeNull();
        }

        private string GetHangfireConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(_msSqlContainer.GetConnectionString())
            {
                InitialCatalog = "Hangfire"
            };
            return builder.ConnectionString;
        }
    }
}