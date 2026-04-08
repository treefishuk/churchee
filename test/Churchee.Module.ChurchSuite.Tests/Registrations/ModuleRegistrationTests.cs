using Churchee.Module.ChurchSuite.Registrations;

namespace Churchee.Module.ChurchSuite.Tests.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_Dashboard()
        {
            var cut = new ModuleRegistration();
            Assert.Equal("ChurchSuite", cut.Name);
        }
    }
}
