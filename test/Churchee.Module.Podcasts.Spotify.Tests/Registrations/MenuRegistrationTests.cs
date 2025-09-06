using Churchee.Module.Podcasts.Spotify.Registrations;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Podcasts.Spotify.Tests.Registrations
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
            mainMenuItem.Children.Should().HaveCount(1);


            mainMenuItem.Children.Should().ContainSingle();
            var childMenuItem = mainMenuItem.Children.First();
            childMenuItem.Name.Should().Be("Spotify Podcasts");
            childMenuItem.Path.Should().Be("/management/integrations/spotify");
            childMenuItem.Icon.Should().Be("graphic_eq");
            childMenuItem.Order.Should().Be(1);
        }
    }

}
