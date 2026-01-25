using Churchee.Module.Logging.Extensions;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Churchee.Module.Logging.Tests.Extensions
{
    public class LoggerConfigurationExtensionTests
    {

        [Fact]
        public void AddTelegramSinkIfConfigured_Should_ReturnLogger()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"Logging:Telegram:BotToken", "test-bot-token"},
                {"Logging:Telegram:Enabled", "true"},
                {"Logging:Telegram:ChatId", "test-chat-id"}
            };
            configurationBuilder.AddInMemoryCollection(inMemorySettings);
            var configuration = configurationBuilder.Build();

            var loggerConfiguration = new LoggerConfiguration();

            // Act
            loggerConfiguration = loggerConfiguration.AddTelegramSinkIfConfigured(configuration);

            var logger = loggerConfiguration.CreateLogger();

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        public void AddTelegramSinkIfNotConfigured_ShouldStill_ReturnLogger()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();
            var inMemorySettings = new Dictionary<string, string?>();
            configurationBuilder.AddInMemoryCollection(inMemorySettings);
            var configuration = configurationBuilder.Build();

            var loggerConfiguration = new LoggerConfiguration();

            // Act
            loggerConfiguration = loggerConfiguration.AddTelegramSinkIfConfigured(configuration);

            var logger = loggerConfiguration.CreateLogger();

            // Assert
            Assert.NotNull(logger);
        }
    }
}
