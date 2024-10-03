using Churchee.Module.Events.Registration;
using FluentAssertions;
namespace Churchee.Module.Events.Tests.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_Events()
        {
            var cut = new ModuleRegistration();
            cut.Name.Should().Be("Events");
        }
    }
}
