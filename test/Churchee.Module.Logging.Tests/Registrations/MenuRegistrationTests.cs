using Churchee.Module.Logging.Registration;

namespace Churchee.Module.Logging.Tests.Registrations
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

            Assert.Equal("Configuration", mainMenuItem?.Name);
            Assert.Equal("/management/configuration", mainMenuItem?.Path);
            Assert.Equal("settings", mainMenuItem?.Icon);
            Assert.Equal(1000, mainMenuItem?.Order);
            Assert.Equal(1, mainMenuItem?.Children.Count);

            var childMenuItem = mainMenuItem?.Children.FirstOrDefault();
            Assert.Equal("Logs", childMenuItem?.Name);
            Assert.Equal("/management/configuration/logs", childMenuItem?.Path);
            Assert.Equal("list", childMenuItem?.Icon);
            Assert.Equal(100, childMenuItem?.Order);
            Assert.Equal(0, childMenuItem?.Children.Count);

        }
    }

}
