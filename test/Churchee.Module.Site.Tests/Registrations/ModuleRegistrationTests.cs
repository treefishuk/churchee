using Churchee.Test.Helpers.Validation;
namespace Churchee.Module.Site.Test.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_Site()
        {
            var cut = new ModuleRegistration();
            cut.Name.Should().Be("Site");
        }
    }
}
