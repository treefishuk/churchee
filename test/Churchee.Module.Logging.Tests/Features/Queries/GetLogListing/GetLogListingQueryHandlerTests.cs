using Churchee.Module.Logging.Entities;
using Churchee.Module.Logging.Features.Queries;
using Churchee.Module.Logging.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Logging.Tests.Features.Queries
{
    public class GetLogListingQueryHandlerTests
    {
        private static LogsDBContext CreateContextWithLogs(IEnumerable<Log> logs)
        {
            var options = new DbContextOptionsBuilder<LogsDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new LogsDBContext(options);

            context.Set<Log>().AddRange(logs);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task Handle_ReturnsAllLogs_WhenNoSkipOrTake()
        {
            // Arrange
            var logs = new List<Log>
            {
                new() { Id = 1, Message = "A", Level = "Info", TimeStamp = new DateTime(2024, 1, 1) },
                new() { Id = 2, Message = "B", Level = "Warning", TimeStamp = new DateTime(2024, 1, 2) }
            };
            using var context = CreateContextWithLogs(logs);
            var handler = new GetLogListingQueryHandler(context);
            var query = new GetLogListingQuery(0, 10, null, "Id") { OrderByDirection = "asc" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.RecordsTotal);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal(10, result.Draw);
            Assert.Contains(result.Data, x => x.Message == "A");
            Assert.Contains(result.Data, x => x.Message == "B");
        }

        [Fact]
        public async Task Handle_RespectsSkipAndTake()
        {
            // Arrange
            var logs = new List<Log>
            {
                new() { Id = 1, Message = "A", Level = "Info", TimeStamp = new DateTime(2024, 1, 1) },
                new() { Id = 2, Message = "B", Level = "Warning", TimeStamp = new DateTime(2024, 1, 2) },
                new() { Id = 3, Message = "C", Level = "Error", TimeStamp = new DateTime(2024, 1, 3) }
            };
            using var context = CreateContextWithLogs(logs);
            var handler = new GetLogListingQueryHandler(context);
            var query = new GetLogListingQuery(1, 1, null, "Id") { OrderByDirection = "asc" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(3, result.RecordsTotal);
            Assert.Single(result.Data);
            Assert.Equal("B", result.Data.First().Message);
        }

        [Fact]
        public async Task Handle_OrdersDescending()
        {
            // Arrange
            var logs = new List<Log>
            {
                new() { Id = 1, Message = "A", Level = "Info", TimeStamp = new DateTime(2024, 1, 1) },
                new() { Id = 2, Message = "B", Level = "Warning", TimeStamp = new DateTime(2024, 1, 2) }
            };
            using var context = CreateContextWithLogs(logs);
            var handler = new GetLogListingQueryHandler(context);
            var query = new GetLogListingQuery(0, 2, null, "Id") { OrderByDirection = "desc" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.RecordsTotal);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal("B", result.Data.First().Message);
        }

        [Fact]
        public async Task Handle_ReturnsEmpty_WhenNoLogs()
        {
            // Arrange
            using var context = CreateContextWithLogs(new List<Log>());
            var handler = new GetLogListingQueryHandler(context);
            var query = new GetLogListingQuery(0, 10, null, "Id") { OrderByDirection = "asc" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(0, result.RecordsTotal);
            Assert.Empty(result.Data);
        }
    }
}
