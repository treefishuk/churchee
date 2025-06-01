using Churchee.Module.Logging.Entities;
using Churchee.Module.Logging.Features.Queries;
using Churchee.Module.Logging.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Logging.Tests.Features.Queries
{
    public class GetLogDetailsQueryHandlerTests
    {
        private LogsDBContext CreateContextWithLog(Log log = null)
        {
            var options = new DbContextOptionsBuilder<LogsDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new LogsDBContext(options);

            if (log != null)
            {
                context.Set<Log>().Add(log);
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public async Task Handle_ReturnsLogDetails_WhenLogExists()
        {
            // Arrange
            var log = new Log
            {
                Id = 1,
                Message = "Test message",
                MessageTemplate = "Template",
                Level = "Warning",
                TimeStamp = new DateTime(2024, 1, 1, 12, 0, 0),
                Exception = "Exception details",
                Properties = "<props/>"
            };

            using var context = CreateContextWithLog(log);
            var handler = new GetLogDetailsQueryHandler(context);
            var query = new GetLogDetailsQuery(1);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(log.Id, result.Id);
            Assert.Equal(log.Message, result.Message);
            Assert.Equal(log.MessageTemplate, result.MessageTemplate);
            Assert.Equal(log.Level, result.Level);
            Assert.Equal(log.TimeStamp, result.TimeStamp);
            Assert.Equal(log.Exception, result.Exception);
            Assert.Equal(log.Properties, result.PropertiesString);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenLogDoesNotExist()
        {
            // Arrange
            using var context = CreateContextWithLog();
            var handler = new GetLogDetailsQueryHandler(context);
            var query = new GetLogDetailsQuery(999);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
