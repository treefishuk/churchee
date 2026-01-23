using Churchee.Module.Podcasts.Spotify.Registrations;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Podcasts.Spotify.Tests.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_ShouldReturnExpectedModuleName()
        {
            // Arrange
            var moduleRegistration = new ModuleRegistration();

            // Act
            string moduleName = moduleRegistration.Name;

            // Assert
            moduleName.Should().NotBeNull();
            moduleName.Should().Be("Spotify");
        }
    }
}
