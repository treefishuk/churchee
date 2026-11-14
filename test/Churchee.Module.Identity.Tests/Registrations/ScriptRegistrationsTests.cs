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
            scripts.Count.Should().Be(2);
            scripts.Any(a => string.Equals(a, "/_content/Churchee.Module.Identity/scripts/qrcode.js", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            scripts.Any(a => string.Equals(a, "/_content/Churchee.Module.Identity/scripts/churchee-identity.js", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }
    }
}
