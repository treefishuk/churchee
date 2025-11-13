using Churchee.Module.YouTube.Registrations;

namespace Churchee.Module.YouTube.Tests.Registrations
{
    public class MenuRegistrationTests
    {
        [Fact]
        public void MenuItems_Returns_List_With_YouTube_Child()
        {
            var registration = new MenuRegistration();

            var items = registration.MenuItems.ToList();

            Assert.NotEmpty(items);
            var root = items.First();
            Assert.Equal("Integrations", root.Name);
            Assert.Contains(root.Children, c => c.Name == "YouTube Videos");
        }
    }
}
