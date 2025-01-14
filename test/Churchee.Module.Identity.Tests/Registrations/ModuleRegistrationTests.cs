using Churchee.Module.Identity.Registrations;
using FluentAssertions;

namespace Churchee.Module.Identity.Tests.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_ShouldReturnExpectedModuleName()
        {
            // Arrange
            var moduleRegistration = new ModuleRegistration();

            // Act
            var moduleName = moduleRegistration.Name;

            // Assert
            moduleName.Should().NotBeNull();
            moduleName.Should().Be("Identity");
        }
    }
}
