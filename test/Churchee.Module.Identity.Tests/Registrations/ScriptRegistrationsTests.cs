using Churchee.Module.Identity.Registrations;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Identity.Tests.Registrations
{
    public class ScriptRegistrationsTests
    {
        [Fact]
        public void ScriptRegistrations_ShouldReturnExpectedScripts()
        {
            // Arrange
            var scriptRegistrations = new ScriptRegistrations();

            // Act
            var scripts = scriptRegistrations.Scripts;

            // Assert
            scripts.Should().NotBeNull();
            scripts.Should().HaveCount(2);
            scripts.Should().Contain("/_content/Churchee.Module.Identity/scripts/qrcode.js");
            scripts.Should().Contain("/_content/Churchee.Module.Identity/scripts/churchee-identity.js");
        }
    }
}
