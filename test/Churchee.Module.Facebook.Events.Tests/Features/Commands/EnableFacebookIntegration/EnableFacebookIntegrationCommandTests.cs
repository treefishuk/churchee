using Churchee.Module.Facebook.Events.Features.Commands;

namespace Churchee.Module.Facebook.Events.Tests.Features.Commands
{
    public class EnableFacebookIntegrationCommandTests
    {
        [Fact]
        public void EnableFacebookIntegrationCommandTests_PropertiesSetByConstructor()
        {
            // Arrange
            string token = "test_token";
            string domain = "test_domain";

            // Act
            var command = new EnableFacebookIntegrationCommand(token, domain);

            // Assert
            Assert.Equal(token, command.Token);
            Assert.Equal(domain, command.Domain);
        }

    }
}
