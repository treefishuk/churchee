using Churchee.Data.EntityFramework.Site.Registrations;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Data.EntityFramework.Site.Tests.Registrations
{
    public class EntityRegistrationTests
    {
        [Fact]
        public void EntityRegistration_Sets_Priority()
        {
            // Arrange

            var cut = new EntityRegistration();

            // Assert
            cut.Priority.Should().Be(1000);
        }
    }
}
