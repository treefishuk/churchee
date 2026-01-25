using Churchee.Module.Logging.Registrations;
using Churchee.Test.Helpers.Extensions;
using Churchee.Test.Helpers.Validation;
using Moq;
using Moq.Protected;
using Serilog.Events;
using Serilog.Parsing;
using System.Net;

namespace Churchee.Module.Logging.Tests.Sinks
{
    public class TelegramSinkTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public TelegramSinkTests()
        {

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        }

        [Fact]
        public void Empty_LogEvent_Does_Nothing()
        {
            // Arrange
            string token = "dummy_token";
            string chatId = "dummy_chat_id";

            var cut = new TestableTelegramSink(token, chatId, _httpClient);

            // Act
            cut.Emit(null);

            // Assert
            cut.BackgroundTaskCalled.Should().BeFalse();
        }

        [Fact]
        public void Info_LogEvent_Does_Nothing()
        {
            // Arrange
            string token = "dummy_token";
            string chatId = "dummy_chat_id";

            var cut = new TestableTelegramSink(token, chatId, _httpClient);

            var logEvent = new LogEvent(
                  DateTimeOffset.UtcNow,
                  LogEventLevel.Information,
                  null,
                  new MessageTemplate("test", []),
                  []);

            // Act
            cut.Emit(logEvent);

            // Assert
            cut.BackgroundTaskCalled.Should().BeFalse();
        }

        [Fact]
        public async Task Error_LogEvent_Calls_Client()
        {
            // Arrange
            string token = "dummy_token";
            string chatId = "dummy_chat_id";

            var cut = new TelegramSink(token, chatId, _httpClient);

            var payload = new
            {
                chat_id = "1234",
                text = "message",
                parse_mode = "HTML",
                disable_web_page_preview = true
            };

            // Act
            await cut.SendPostAsync(payload);

            // Assert
            _mockHttpMessageHandler.VerifyPost("sendMessage", Times.Once());
        }



        [Fact]
        public void SendPostAsync_Calls_Client()
        {
            // Arrange
            string token = "dummy_token";
            string chatId = "dummy_chat_id";

            var cut = new TestableTelegramSink(token, chatId, _httpClient);

            var logEvent = new LogEvent(
                  DateTimeOffset.UtcNow,
                  LogEventLevel.Error,
                  null,
                  new MessageTemplate("test", []),
                  []);

            // Act
            cut.Emit(logEvent);

            // Assert
            cut.BackgroundTaskCalled.Should().BeTrue();
        }

        [Fact]
        public void SendPostAsync__WithException_Calls_Client()
        {
            // Arrange
            string token = "dummy_token";
            string chatId = "dummy_chat_id";

            var cut = new TestableTelegramSink(token, chatId, _httpClient);

            var logEvent = new LogEvent(
                  DateTimeOffset.UtcNow,
                  LogEventLevel.Error,
                  new Exception("Test Exception"),
                  new MessageTemplate("test", []),
                  []);

            // Act
            cut.Emit(logEvent);

            // Assert
            cut.BackgroundTaskCalled.Should().BeTrue();
        }

        [Fact]
        public void SendPostAsync_ErrorSwallowed()
        {
            // Arrange
            string token = "dummy_token";
            string chatId = "dummy_chat_id";

            var cut = new TestableTelegramSink(token, chatId, _httpClient);

            var payload = new
            {
                chat_id = "1234",
                text = "message",
                parse_mode = "HTML",
                disable_web_page_preview = true
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Boom"));

            // Act
            var act = () => cut.SendPostAsync(payload);

            // Assert
            act.Should().NotThrow();

        }

        [Fact]
        public void Returns_RenderedMessage_When_RenderMessage_Is_Not_Empty()
        {
            // Arrange
            var parser = new MessageTemplateParser();
            var template = parser.Parse("Hello {Name}");

            var logEvent = new LogEvent(
                DateTimeOffset.Now,
                LogEventLevel.Information,
                exception: null,
                messageTemplate: template,
                properties: new[]
                {
            new LogEventProperty("Name", new ScalarValue("World"))
                });

            // Act
            string result = TelegramSink.RenderMessage(logEvent);

            // Assert
            Assert.Equal("Hello \"World\"", result);
        }

        [Fact]
        public void Falls_Back_To_MessageTemplate_Text_When_RenderedMessage_Is_Whitespace()
        {
            // Arrange
            var messageTemplate = new MessageTemplate(
                "Fallback text",
                []);

            var logEvent = new LogEvent(
                DateTimeOffset.Now,
                LogEventLevel.Information,
                exception: null,
                messageTemplate: messageTemplate,
                properties: []);

            // Act
            string result = TelegramSink.RenderMessage(logEvent);

            // Assert
            Assert.Equal("Fallback text", result);
        }

        private class TestableTelegramSink : TelegramSink
        {
            public TestableTelegramSink(string botToken, string chatId, HttpClient httpClient)
                : base(botToken, chatId, httpClient)
            {
            }

            public bool BackgroundTaskCalled { get; private set; }

            internal override Task SendFireAndForgetAsync(object payload)
            {
                BackgroundTaskCalled = true;

                return Task.CompletedTask;
            }
        }


    }
}
