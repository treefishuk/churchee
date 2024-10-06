using FluentAssertions;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Churchee.Module.Hangfire.Tests.Extensions
{
    public class HangfireExtensionsTests
    {
        private const string FakeConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Fake;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private const string FakeConnectionString2 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Fake2;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private const string MasterConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        [Fact]
        public void HangfireExtensions_CreateDatabaseIfNotExists_CreatesDatabase_IfNotExist()
        {
            //arrange
            var configurationMock = new Mock<IGlobalConfiguration>();

            DeleteDatabaseIfExists();

            //act
            HangfireExtensions.CreateDatabaseIfNotExists(configurationMock.Object, FakeConnectionString);

            DatabaseExists().Should().BeTrue();

        }


        [Fact]
        public void HangfireExtensions_UseChurcheeHangfireDashboard_SetsRoute()
        {
            //arrange
            var globalConfigurationMock = new Mock<IGlobalConfiguration>();

            var services = new ServiceCollection();

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(FakeConnectionString2, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                }));

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ApplicationBuilder(serviceProvider);

            //act

            HangfireExtensions.UseChurcheeHangfireDashboard(builder);

            //assert builds
            builder.Build();
        }

        private bool DatabaseExists()
        {
            using (SqlConnection connection = new SqlConnection(MasterConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM sys.databases WHERE name = 'Fake'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        private void DeleteDatabaseIfExists()
        {
            string databaseName = "Fake"; // Replace with your actual database name

            using (SqlConnection connection = new SqlConnection(MasterConnectionString))
            {
                connection.Open();

                // First, forcibly close all other connections (if any)
                string killConnectionsQuery = $@"
                    USE [master];
                    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    ALTER DATABASE [{databaseName}] SET MULTI_USER;
                ";

                using (SqlCommand killCommand = new SqlCommand(killConnectionsQuery, connection))
                {
                    killCommand.ExecuteNonQuery();
                }

                // Now drop the database
                string dropDatabaseQuery = $"USE [master]; DROP DATABASE [{databaseName}];";
                using (SqlCommand dropCommand = new SqlCommand(dropDatabaseQuery, connection))
                {
                    dropCommand.ExecuteNonQuery();
                }
            }
        }

    }
}
