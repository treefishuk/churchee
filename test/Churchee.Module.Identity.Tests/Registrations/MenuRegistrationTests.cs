using Churchee.Module.Identity.Registrations;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Identity.Tests.Registrations
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

            var mainMenuItem = menuItems.First();
            mainMenuItem.Name.Should().Be("Configuration");
            mainMenuItem.Path.Should().Be("/management/configuration");
            mainMenuItem.Icon.Should().Be("settings");
            mainMenuItem.Order.Should().Be(1000);
            mainMenuItem.Children.Should().HaveCount(1);

            var childMenuItem = mainMenuItem.Children.First();
            childMenuItem.Name.Should().Be("Contributors");
            childMenuItem.Path.Should().Be("/management/configuration/contributors");
            childMenuItem.Icon.Should().Be("supervisor_account");
            childMenuItem.Order.Should().Be(1);
            childMenuItem.RequiredRole.Should().Be("Admin");
        }
    }

}
