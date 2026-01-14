using Churchee.Infrastructure.AiTools.Settings;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Infrastructure.AiTools.Tests.Settings
{
    public class AzureVisionSettingsTests
    {
        [Fact]
        public void AzureVisionSettings_ShouldHaveExpectedProperties()
        {
            // Arrange & Act
            var settings = new AzureVisionSettings
            {
                Endpoint = "https://example.com",
                ApiKey = "test-api-key"
            };
            // Assert
            settings.Endpoint.Should().Be("https://example.com");
            settings.ApiKey.Should().Be("test-api-key");
        }
    }
}
