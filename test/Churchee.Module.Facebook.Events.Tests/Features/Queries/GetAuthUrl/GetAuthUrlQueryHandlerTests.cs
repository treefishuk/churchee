using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Exceptions;
using Churchee.Common.Storage;
using Churchee.Module.Facebook.Events.Features.Queries;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Churchee.Module.Facebook.Events.Tests.Features.Queries
{
    public class GetAuthUrlQueryHandlerTests
    {
        private readonly Mock<ISettingStore> _settingStoreMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();

        private GetAuthUrlQueryHandler CreateHandler(IConfiguration configuration)
        {
            return new GetAuthUrlQueryHandler(
                configuration,
                _settingStoreMock.Object,
                _currentUserMock.Object
            );
        }

        [Fact]
        public async Task Handle_ReturnsAuthUrl_AndUpdatesSettings()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var domain = "https://example.com";
            var pageId = "page123";
            var facebookAppId = "fb-app-id";
            _currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var inMemorySettings = new System.Collections.Generic.Dictionary<string, string?>
            {
                { "facebookAppId", facebookAppId }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();


            var handler = CreateHandler(configuration);
            var query = new GetAuthUrlQuery(domain, pageId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            _settingStoreMock.Verify(x =>
                x.AddOrUpdateSetting(
                    Guid.Parse("3de048ae-d711-4609-9b66-97564a9d0d68"),
                    tenantId,
                    "FacebookPageId",
                    pageId), Times.Once);

            _settingStoreMock.Verify(x =>
                x.AddOrUpdateSetting(
                    Guid.Parse("841fb9d0-92ca-41b2-9cdb-5903a6ab7bad"),
                    tenantId,
                    "FacebookStateId",
                    It.IsAny<string>()), Times.Once);

            Assert.StartsWith($"https://www.facebook.com/v18.0/dialog/oauth?client_id={facebookAppId}&redirect_uri={domain}/management/integrations/facebook-events/auth?state=", result);
        }

        [Fact]
        public async Task Handle_ThrowsMissingConfirgurationSettingException_WhenAppIdMissing()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var inMemorySettings = new System.Collections.Generic.Dictionary<string, string?>
            {
                { "facebookAppId", string.Empty }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var handler = CreateHandler(configuration);
            var query = new GetAuthUrlQuery("https://example.com", "page123");

            // Act & Assert
            await Assert.ThrowsAsync<MissingConfigurationSettingException>(() =>
                handler.Handle(query, CancellationToken.None));
        }
    }
}
