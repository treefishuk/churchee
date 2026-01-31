using Churchee.Module.Jotform.Registrations;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Jotform.Tests.Registrations
{
    public class MenuRegistrationTests
    {

        [Fact]
        public void MenuItems_ShouldReturnExpectedMenuItems()
        {
            // Arrange
            var menuRegistration = new MenuRegistration();

            // Act
            var menuItems = menuRegistration.MenuItems;

            // Assert
            menuItems.Should().NotBeNull();
            menuItems.Should().HaveCount(1);
            menuItems.Should().ContainSingle();

            var mainMenuItem = menuItems.First();
            mainMenuItem.Name.Should().Be("Integrations");
            mainMenuItem.Path.Should().Be("/management/integrations");
            mainMenuItem.Icon.Should().Be("integration_instructions");
            mainMenuItem.Order.Should().Be(100);
        }
    }

}
