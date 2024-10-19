using Churchee.Module.Site.Entities;
using FluentAssertions;

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
