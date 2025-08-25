using Churchee.Data.EntityFramework.Site.Registrations;
using Churchee.Module.Tenancy.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Testcontainers.MsSql;

namespace Churchee.Data.EntityFramework.Site.Tests.Registrations
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
        public void ServiceRegistrations_ShouldReturnExpectedServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Mock Logger
            var logger = new Mock<ILogger<SiteDbContext>>();
            services.AddSingleton(logger.Object);

            // Mock Tenant Resolver
            var tenantResolver = new Mock<ITenantResolver>();
            tenantResolver.Setup(tr => tr.GetTenantId()).Returns(Guid.NewGuid());
            services.AddSingleton(tenantResolver.Object);

            // Mock Configuration
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(s => s.GetSection("ConnectionStrings")["Default"]).Returns(_msSqlContainer.GetConnectionString());
            services.AddSingleton(mockConfiguration.Object);

            var serviceRegistrations = new ServiceRegistrations();

            var serviceProvider = services.BuildServiceProvider();

            // Act
            serviceRegistrations.Execute(services, serviceProvider);

            serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetService<SiteDbContext>().Should().NotBeNull();
            serviceProvider.GetService<DbContext>().Should().NotBeNull();
        }
    }
}
