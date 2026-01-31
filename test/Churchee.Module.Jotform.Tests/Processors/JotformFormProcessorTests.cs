using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Jotform.Processors;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Churchee.Test.Helpers;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace Churchee.Module.Jotform.Tests.Processors
{
    public class JotformFormProcessorTests
    {

        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<IDataStore> _mockDataStore;
        private readonly Mock<ILogger<JotformFormProcessor>> _mockLogger;
        private readonly Mock<IRepository<Token>> _mockTokenRepository;

        public JotformFormProcessorTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<JotformFormProcessor>>();
            _mockDataStore = new Mock<IDataStore>();
            _mockTokenRepository = new Mock<IRepository<Token>>();
            _mockDataStore.Setup(s => s.GetRepository<Token>()).Returns(_mockTokenRepository.Object);
        }

        [Fact]
        public async Task JotformFormProcessor_ProcessForm_FormId_Invalid_Returns_False()
        {
            // Arrange & Act
            var inMemorySettings = new Dictionary<string, string?> {
                {"Jotform:Api", "https://api.jotform.com/"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var cut = new JotformFormProcessor(_mockHttpClientFactory.Object, _mockDataStore.Object, _mockLogger.Object, configuration);

            var formData = new Dictionary<string, string>();

            // Act
            bool response = await cut.ProcessForm(formData);

            // Assert
            response.Should().BeFalse();
        }

        [Fact]
        public async Task JotformFormProcessor_ProcessForm_FormId_Not_Int_Returns_False()
        {
            // Arrange & Act
            var inMemorySettings = new Dictionary<string, string?> {
                {"Jotform:Api", "https://api.jotform.com/"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var cut = new JotformFormProcessor(_mockHttpClientFactory.Object, _mockDataStore.Object, _mockLogger.Object, configuration);

            var formData = new Dictionary<string, string>()
            {
                { "formID", "NotAnInt"}
            };

            // Act
            bool response = await cut.ProcessForm(formData);

            // Assert
            response.Should().BeFalse();
        }

        [Fact]
        public async Task JotformFormProcessor_ProcessForm_ApiKey_Null_Returns_False()
        {
            // Arrange & Act
            var inMemorySettings = new Dictionary<string, string?> {
                {"Jotform:Api", "https://api.jotform.com/"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.Forbidden));

            _mockHttpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new JotformFormProcessor(_mockHttpClientFactory.Object, _mockDataStore.Object, _mockLogger.Object, configuration);

            var formData = new Dictionary<string, string>()
            {
                { "formID", "1234567890"},
                {  "q1", "Test"  }
            };

            // Act
            bool response = await cut.ProcessForm(formData);

            // Assert
            response.Should().BeFalse();
        }

        [Fact]
        public async Task JotformFormProcessor_ProcessForm_ApiKey_EmptyString_Returns_False()
        {
            // Arrange & Act
            var inMemorySettings = new Dictionary<string, string?> {
                {"Jotform:Api", "https://api.jotform.com/"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.Forbidden));

            _mockHttpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            _mockTokenRepository.Setup(s => s.FirstOrDefaultAsync(It.IsAny<GetTokenByKeySpecification>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            var cut = new JotformFormProcessor(_mockHttpClientFactory.Object, _mockDataStore.Object, _mockLogger.Object, configuration);

            var formData = new Dictionary<string, string>()
            {
                { "formID", "1234567890"},
                {  "q1", "Test"  }
            };

            // Act
            bool response = await cut.ProcessForm(formData);

            // Assert
            response.Should().BeFalse();
        }


        [Fact]
        public async Task JotformFormProcessor_ProcessForm_ApiPost_Fails_Returns_False()
        {
            // Arrange & Act
            var inMemorySettings = new Dictionary<string, string?> {
                {"Jotform:Api", "https://api.jotform.com/"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.Forbidden));

            _mockHttpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            _mockTokenRepository.Setup(s => s.FirstOrDefaultAsync(It.IsAny<GetTokenByKeySpecification>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Api-Key");

            var cut = new JotformFormProcessor(_mockHttpClientFactory.Object, _mockDataStore.Object, _mockLogger.Object, configuration);

            var formData = new Dictionary<string, string>()
            {
                { "formID", "1234567890"},
                {  "q1", "Test"  }
            };

            _mockLogger.Setup(s => s.IsEnabled(LogLevel.Warning)).Returns(true);

            // Act
            bool response = await cut.ProcessForm(formData);

            // Assert
            response.Should().BeFalse();
            _mockLogger.Verify(l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Jotform submission failed")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        [Fact]
        public async Task JotformFormProcessor_ProcessForm_ApiPost_Success_Returns_True()
        {
            // Arrange & Act
            var inMemorySettings = new Dictionary<string, string?> {
                {"Jotform:Api", "https://api.jotform.com/"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK));

            _mockHttpClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            _mockTokenRepository.Setup(s => s.FirstOrDefaultAsync(It.IsAny<GetTokenByKeySpecification>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Api-Key");

            var cut = new JotformFormProcessor(_mockHttpClientFactory.Object, _mockDataStore.Object, _mockLogger.Object, configuration);

            var formData = new Dictionary<string, string>()
            {
                { "formID", "1234567890"},
                {  "q1", "Test"  }
            };

            _mockLogger.Setup(s => s.IsEnabled(LogLevel.Warning)).Returns(true);

            // Act
            bool response = await cut.ProcessForm(formData);

            // Assert
            response.Should().BeTrue();

        }
    }
}
