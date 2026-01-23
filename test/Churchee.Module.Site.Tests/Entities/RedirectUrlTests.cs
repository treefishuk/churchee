using Churchee.Module.Site.Entities;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Site.Tests.Entities
{
    public class RedirectUrlTests
    {
        [Fact]
        public void PageTypeProperty_Should_HaveCorrectProperties()
        {
            // Arrange
            var redirect = new RedirectUrl("/path");

            // Act & Assert
            redirect.Path.Should().Be("/path");
        }
    }
}
