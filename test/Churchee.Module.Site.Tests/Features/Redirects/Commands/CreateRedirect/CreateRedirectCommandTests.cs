using Churchee.Module.Site.Features.Redirects.Commands;

namespace Churchee.Module.Site.Tests.Features.Redirects.Commands
{
    public class CreateRedirectCommandTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            string path = "/test";
            var webContentId = Guid.NewGuid();

            // Act
            var command = new CreateRedirectCommand(path, webContentId);

            // Assert
            Assert.Equal(path, command.Path);
            Assert.Equal(webContentId, command.WebContentId);
        }
    }
}
