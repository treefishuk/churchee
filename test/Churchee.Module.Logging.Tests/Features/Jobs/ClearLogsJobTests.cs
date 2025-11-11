using Churchee.Module.Logging.Entities;
using Churchee.Module.Logging.Infrastructure;
using Churchee.Module.Logging.Jobs;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Churchee.Module.Logging.Tests.Jobs
{
    public class ClearLogsJobTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;

        public ClearLogsJobTests()
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
        public async Task ExecuteAsync_RemovesLogsOlderThan30Days()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LogsDBContext>()
                .UseSqlServer(_msSqlContainer.GetConnectionString())
                .Options;

            using var dbContext = new LogsDBContext(options);

            dbContext.Database.EnsureCreated();

            // Add logs: one older than 30 days, one newer
            var oldLog = new Log { TimeStamp = DateTime.UtcNow.AddDays(-31) };
            var newLog = new Log { TimeStamp = DateTime.UtcNow.AddDays(-10) };
            dbContext.Set<Log>().AddRange(oldLog, newLog);
            await dbContext.SaveChangesAsync();

            var job = new ClearLogsJob(dbContext);

            // Act
            await job.ExecuteAsync(CancellationToken.None);

            // Assert
            var logs = dbContext.Set<Log>().ToList();
            Assert.DoesNotContain(logs, l => l.TimeStamp < DateTime.UtcNow.AddDays(-30));
            Assert.Contains(logs, l => l.TimeStamp > DateTime.UtcNow.AddDays(-30));
        }
    }

}