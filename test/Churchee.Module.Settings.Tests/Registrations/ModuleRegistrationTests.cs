using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Settings.Tests.Registrations
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
            moduleName.Should().Be("Settings");
        }
    }
}
