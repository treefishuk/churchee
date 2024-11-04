using Churchee.Module.Dashboard.Registrations;
using FluentAssertions;
namespace Churchee.Module.Dashboard.Tests.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_Dashboard()
        {
            var cut = new ModuleRegistration();
            cut.Name.Should().Be("Dashboard");
        }
    }
}
