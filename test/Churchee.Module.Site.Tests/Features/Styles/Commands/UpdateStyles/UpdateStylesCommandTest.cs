using Churchee.Module.Site.Features.Styles.Commands;

namespace Churchee.Module.Site.Tests.Features.Styles.Commands.UpdateStyles
{
    public class UpdateStylesCommandTest
    {
        [Fact]
        public void Constructor_ShouldInitializeCssProperty()
        {
            // Arrange
            var css = "body { background-color: red; }";

            // Act
            var command = new UpdateStylesCommand(css);

            // Assert
            Assert.Equal(css, command.Css);
        }

        [Fact]
        public void CssProperty_ShouldReturnInitializedValue()
        {
            // Arrange
            var css = "body { background-color: blue; }";
            var command = new UpdateStylesCommand(css);

            // Act
            var result = command.Css;

            // Assert
            Assert.Equal(css, result);
        }
    }
}
