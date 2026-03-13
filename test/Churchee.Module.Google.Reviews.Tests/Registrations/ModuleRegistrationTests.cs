using Churchee.Module.Google.Reviews.Registrations;
using Churchee.Test.Helpers.Validation;
namespace Churchee.Module.Google.Reviews.Test.Registrations
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void ModuleRegistration_Returns_GoogleReviews()
        {
            var cut = new ModuleRegistration();
            cut.Name.Should().Be("GoogleReviews");
        }
    }
}
