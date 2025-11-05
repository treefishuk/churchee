using Churchee.Common.Storage;
using Churchee.Module.Site.Registration;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Site.Tests.Registrations
{
    public class MenuRegistrationTests
    {
        [Fact]
        public void MenuItems_ShouldReturnExpectedMenuItems()
        {
            // Arrange
            var mockDataStore = new Mock<IDataStore>();

            var menuRegistration = new MenuRegistration(mockDataStore.Object);

            // Act
            var menuItems = menuRegistration.MenuItems;

            // Assert
            menuItems.Should().NotBeNull();
            menuItems.Should().HaveCount(1);
            menuItems.Should().ContainSingle();

            var mainMenuItem = menuItems.First();
            mainMenuItem.Name.Should().Be("Website");
            mainMenuItem.Path.Should().Be("/management/pages");
            mainMenuItem.Icon.Should().Be("devices");
            mainMenuItem.Order.Should().Be(100);
            mainMenuItem.Children.Count.Should().Be(10);

            var pagesMenuItem = mainMenuItem.Children[0];
            pagesMenuItem.Name.Should().Be("Pages");
            pagesMenuItem.Path.Should().Be("/management/pages");
            pagesMenuItem.Icon.Should().Be("web");
            pagesMenuItem.Order.Should().Be(1);
        }
    }

}
