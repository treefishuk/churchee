using Churchee.Module.X.Registrations;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.X.Tests.Registrations
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
            moduleName.Should().Be("X");
        }
    }
}
