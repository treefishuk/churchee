using Churchee.Module.Facebook.Events.Registrations;

namespace Churchee.Module.Facebook.Events.Tests.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_Dashboard()
        {
            var cut = new ModuleRegistration();
            Assert.Equal("Facebook", cut.Name);
        }
    }
}
