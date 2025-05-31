using Churchee.Module.Facebook.Events.Registrations;

namespace Churchee.Module.Facebook.Events.Tests.Registrations
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
            Assert.NotNull(menuItems);
            Assert.Single(menuItems);

            var mainMenuItem = menuItems.FirstOrDefault();

            Assert.Equal("Integrations", mainMenuItem?.Name);
            Assert.Equal("/management/integrations", mainMenuItem?.Path);
            Assert.Equal("integration_instructions", mainMenuItem?.Icon);
            Assert.Equal(100, mainMenuItem?.Order);
            Assert.Equal(1, mainMenuItem?.Children.Count);

            var childMenuItem = mainMenuItem?.Children.FirstOrDefault();
            Assert.Equal("Facebook Events", childMenuItem?.Name);
            Assert.Equal("/management/integrations/facebook-events", childMenuItem?.Path);
            Assert.Equal("event", childMenuItem?.Icon);
            Assert.Equal(1, childMenuItem?.Order);
            Assert.Equal(0, childMenuItem?.Children.Count);

        }
    }

}
