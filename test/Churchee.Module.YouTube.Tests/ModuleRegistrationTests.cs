using Xunit;
using Churchee.Module.YouTube.Registrations;

namespace Churchee.Module.YouTube.Tests
{
    public class ModuleRegistrationTests
    {
        [Fact]
        public void Name_Returns_YouTube()
        {
            var registration = new ModuleRegistration();

            Assert.Equal("YouTube", registration.Name);
        }
    }
}
