using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Exceptions;
using Churchee.Common.Storage;
using Churchee.Module.Facebook.Events.Features.Commands;
using Churchee.Module.Facebook.Events.Helpers;
using Churchee.Module.Tokens.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace Churchee.Module.Facebook.Events.Tests.Features.Commands
{
    public class EnableFacebookIntegrationCommandHandlerTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly Mock<ISettingStore> _settingStoreMock = new();
        private readonly Mock<IDataStore> _dataStoreMock = new();
        private readonly Mock<IRepository<Token>> _tokenRepoMock = new();
        private readonly Mock<ILogger<EnableFacebookIntegrationCommandHandler>> _loggerMock = new();

        private readonly Guid _tenantId = Guid.NewGuid();

        private EnableFacebookIntegrationCommandHandler CreateHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            _dataStoreMock.Setup(d => d.GetRepository<Token>()).Returns(_tokenRepoMock.Object);
            _currentUserMock.Setup(u => u.GetApplicationTenantId()).ReturnsAsync(_tenantId);


            return new EnableFacebookIntegrationCommandHandler(
                _httpClientFactoryMock.Object,
                _currentUserMock.Object,
                _settingStoreMock.Object,
                configuration,
                _dataStoreMock.Object,
                _loggerMock.Object
            );
        }

        private static IConfiguration SetUpOptions(Dictionary<string, string?> options)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(options)
                .Build();
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenFacebookApiUrlMissing()
        {
            // Arrange
            var configuration = SetUpOptions(new Dictionary<string, string?> {
                { "facebookAppId", "appid" },
                { "facebookAppSecret", "secret" }
            });

            var handler = CreateHandler(new HttpClient(), configuration);
            var command = new EnableFacebookIntegrationCommand("token", "https://domain.com");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Description == "Missing Facebook API Url Setting");
        }

        [Fact]
        public async Task Handle_ThrowsMissingConfirgurationSettingException_WhenConfigMissing()
        {

            // Arrange
            var configuration = SetUpOptions(new Dictionary<string, string?> {
                { "Facebook:Api", "https://facebook.api/" },
                { "facebookAppId", null }
            });

            var handler = CreateHandler(new HttpClient(), configuration);
            var command = new EnableFacebookIntegrationCommand("token", "https://domain.com");

            // Act & Assert
            await Assert.ThrowsAsync<MissingConfirgurationSettingException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ReturnsEmptyResponse_WhenAccessTokenNull()
        {

            // Arrange
            var configuration = SetUpOptions(new Dictionary<string, string?> {
                { "Facebook:Api", "https://facebook.api/" },
                { "facebookAppId", "appid" },
                { "facebookAppSecret", "secret" }
            });

            _settingStoreMock.Setup(s => s.GetSettingValue(It.IsAny<Guid>(), _tenantId)).ReturnsAsync("pageid");
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("") // Simulate empty response for access token
                });
            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://facebook.api/")
            };

            var handler = CreateHandler(httpClient, configuration);
            var command = new EnableFacebookIntegrationCommand("token", "https://domain.com");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess); // CommandResponse is empty, IsSuccess default is true
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Handle_SuccessfulIntegration_FullFlow()
        {

            // Arrange
            var configuration = SetUpOptions(new Dictionary<string, string?> {
               { "Facebook:Api", "https://facebook.api/" },
               { "facebookAppId", "appid" },
               { "facebookAppSecret", "secret" }
            });

            _settingStoreMock.SetupSequence(s => s.GetSettingValue(It.IsAny<Guid>(), _tenantId))
                .ReturnsAsync("pageid") // pageId
                .ReturnsAsync("stateid"); // stateId

            var accessTokenJson = "{\"access_token\":\"access123\",\"token_type\":\"bearer\",\"expires_in\":3600}";
            var userIdJson = "{\"id\":\"user123\"}";
            var pagesJson = "{\"data\":[{\"id\":\"pageid\",\"access_token\":\"pagetoken123\"}]}";

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(accessTokenJson)
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(userIdJson)
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(pagesJson)
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://facebook.api/")
            };

            var handler = CreateHandler(httpClient, configuration);
            var command = new EnableFacebookIntegrationCommand("token", "https://domain.com");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _tokenRepoMock.Verify(r => r.Create(It.Is<Token>(t => t.Key == SettingKeys.FacebookAccessToken && t.Value == "access123")), Times.Once);
            _tokenRepoMock.Verify(r => r.Create(It.Is<Token>(t => t.Key == SettingKeys.FacebookPageAccessToken && t.Value == "pagetoken123")), Times.Once);
            _settingStoreMock.Verify(s => s.AddOrUpdateSetting(It.IsAny<Guid>(), _tenantId, "FacebookUserId", "user123"), Times.Once);
            _dataStoreMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }
    }
}
