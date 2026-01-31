using Churchee.Module.Jotform.Features.Commands;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Jotform.Tests.Features.Commands
{
    public class ConfigureJotformCommandTests
    {
        [Fact]
        public void ConfigureJotformCommand_Constructor_Sets_ApiKey()
        {
            // Arrange & Act
            var cut = new ConfigureJotformCommand("test-api-key");

            // Assert
            cut.ApiKey.Should().Be("test-api-key");
        }
    }
}
