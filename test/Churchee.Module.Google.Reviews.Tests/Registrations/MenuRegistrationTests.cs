using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Google.Reviews.Registrations
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

            var pagesMenuItem = mainMenuItem.Children[0];
            pagesMenuItem.Name.Should().Be("Google Reviews");
            pagesMenuItem.Path.Should().Be("/management/integrations/google-reviews");
            pagesMenuItem.Icon.Should().Be("hotel_class");
            pagesMenuItem.Order.Should().Be(1);
        }
    }

}
