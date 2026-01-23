using Churchee.Module.Logging.Registrations;

namespace Churchee.Module.Logging.Tests.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_Events()
        {
            var cut = new ModuleRegistration();
            Assert.Equal("Logging", cut.Name);
        }
    }
}
