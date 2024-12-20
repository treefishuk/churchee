using Churchee.Module.Podcasts.Registrations;
using FluentAssertions;
namespace Churchee.Module.Podcasts.Tests.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_Dashboard()
        {
            var cut = new ModuleRegistration();
            cut.Name.Should().Be("Podcasts");
        }
    }
}
