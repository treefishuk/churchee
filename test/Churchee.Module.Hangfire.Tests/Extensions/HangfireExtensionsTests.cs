using Churchee.Test.Helpers.Validation;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Data.Common;
using System.Net;
using Testcontainers.MsSql;

namespace Churchee.Module.Hangfire.Tests.Extensions
{
    public class HangfireExtensionsTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;

        public HangfireExtensionsTests()
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
        public async Task HangfireExtensions_CreateDatabaseIfNotExists_CreatesDatabase_IfNotExist()
        {
            //arrange
            var configurationMock = new Mock<IGlobalConfiguration>();

            //act
            HangfireExtensions.CreateDatabaseIfNotExists(configurationMock.Object, GetHangfireConnectionString());

            var exists = await DatabaseExists();

            exists.Should().BeTrue();

        }


        [Fact]
        public async Task HangfireExtensions_UseChurcheeHangfireDashboard_SetsRoute()
        {
            // arrange
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddHangfire(configuration => configuration
                                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                .UseSimpleAssemblyNameTypeSerializer()
                                .UseRecommendedSerializerSettings()
                                .UseSqlServerStorage(GetHangfireConnectionString(), new SqlServerStorageOptions
                                {
                                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                    QueuePollInterval = TimeSpan.Zero,
                                    UseRecommendedIsolationLevel = true,
                                    DisableGlobalLocks = true,
                                }));

                        })
                        .Configure(app =>
                        {
                            HangfireExtensions.UseChurcheeHangfireDashboard(app);
                        });
                })
                .StartAsync();

            // act
            var response = await host.GetTestClient().GetAsync("/jobs");

            // assert
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);

        }



        private async Task<bool> DatabaseExists()
        {
            using DbConnection connection = new SqlConnection(_msSqlContainer.GetConnectionString());
            await connection.OpenAsync();

            using DbCommand command = connection.CreateCommand();

            command.CommandText = "SELECT database_id FROM sys.databases WHERE name = 'Hangfire'";

            var result = await command.ExecuteScalarAsync();

            return result != null;
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
