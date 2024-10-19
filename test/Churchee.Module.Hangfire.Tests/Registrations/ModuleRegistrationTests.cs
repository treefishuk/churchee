using Churchee.Module.Hangfire.Registrations;
using FluentAssertions;

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
