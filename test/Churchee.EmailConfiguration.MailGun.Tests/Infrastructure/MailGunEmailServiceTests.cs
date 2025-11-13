using Churchee.EmailConfiguration.MailGun.Infrastructure;
using Churchee.Test.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace Churchee.EmailConfiguration.MailGun.Tests.Infrastructure
{
    public class MailGunEmailServiceTests
    {
        private readonly Mock<ILogger<MailGunEmailService>> _loggerMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

        public MailGunEmailServiceTests()
        {
            _loggerMock = new Mock<ILogger<MailGunEmailService>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task SendEmailAsync_SuccessfulRequest_DoesNotLogError()
        {
            // Arrange
            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactoryMock.Setup(f => f.CreateClient("MailGun")).Returns(httpClient);

            var service = new MailGunEmailService(_loggerMock.Object, SetUpValidOptions(), _httpClientFactoryMock.Object);

            // Act
            await service.SendEmailAsync("to@example.com", "To Name", "Subject", "<b>html</b>", "plain");

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        [Fact]
        public async Task SendEmailAsync_HttpClientReturnsError_LogsError()
        {
            // Arrange
            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.NotFound))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactoryMock.Setup(f => f.CreateClient("MailGun")).Returns(httpClient);

            var service = new MailGunEmailService(_loggerMock.Object, SetUpValidOptions(), _httpClientFactoryMock.Object);

            // Act
            await service.SendEmailAsync("to@example.com", "To Name", "Subject", "<b>html</b>", "plain");

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }



        private static IConfiguration SetUpInValidOptions()
        {
            var configForSmsApi = new Dictionary<string, string?>
            {
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configForSmsApi)
                .Build();
        }

        private static IConfiguration SetUpValidOptions()
        {
            var configForSmsApi = new Dictionary<string, string?>
            {
                {"MailGunOptions:APIKey", "key"},
                {"MailGunOptions:FromName", "Name"},
                {"MailGunOptions:FromEmail", "noreply@example.com"},
                {"MailGunOptions:Domain", "example.com"},
                {"MailGunOptions:BaseUrl", "http://localhost/"},
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configForSmsApi)
                .Build();
        }

        [Fact]
        public async Task SendEmailAsync_FailedRequest_LogsError()
        {
            // Arrange
            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.BadRequest));

            _httpClientFactoryMock.Setup(f => f.CreateClient("MailGun")).Returns(httpClient);

            var service = new MailGunEmailService(_loggerMock.Object, SetUpInValidOptions(), _httpClientFactoryMock.Object);

            // Act
            await service.SendEmailAsync("to@example.com", "To Name", "Subject", "<b>html</b>", "plain");

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendEmailAsync_MissingConfig_LogsError()
        {
            // Arrange
            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK));

            _httpClientFactoryMock.Setup(f => f.CreateClient("MailGun")).Returns(httpClient);

            var service = new MailGunEmailService(_loggerMock.Object, SetUpInValidOptions(), _httpClientFactoryMock.Object);

            // Act
            await service.SendEmailAsync("to@example.com", "To Name", "Subject", "<b>html</b>", "plain");

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
