using Churchee.Module.Tokens.Registrations;
using FluentAssertions;
using Xunit;
namespace Churchee.Module.Tokens.Test.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_Events()
        {
            var cut = new ModuleRegistration();
            cut.Name.Should().Be("Tokens");
        }
    }
}
