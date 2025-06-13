using Churchee.Module.Facebook.Events.Features.Commands;

namespace Churchee.Module.Facebook.Events.Tests.Features.Commands
{
    public class EnableFacebookIntegrationCommandTests
    {
        public void EnableFacebookIntegrationCommandTests_PropertiesSetByConstructor()
        {
            // Arrange
            var token = "test_token";
            var domain = "test_domain";

            // Act
            var command = new EnableFacebookIntegrationCommand(token, domain);

            // Assert
            Assert.Equal(token, command.Token);
            Assert.Equal(domain, command.Domain);
        }

    }
}
