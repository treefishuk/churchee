using Churchee.Module.Hangfire.Registrations;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Hangfire.Tests.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_Events()
        {
            var cut = new ModuleRegistration();
            cut.Name.Should().Be("Hangfire");
        }
    }


}
