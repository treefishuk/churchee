using Churchee.Module.X.Registrations;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.X.Tests.Registrations
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
            mainMenuItem.Children.Count.Should().Be(1);

            var childMenuItem = mainMenuItem.Children.First();
            childMenuItem.Name.Should().Be("X/Twitter Sync");
            childMenuItem.Path.Should().Be("/management/integrations/x");
            childMenuItem.Icon.Should().Be("message");
            childMenuItem.Order.Should().Be(1);
        }
    }

}
