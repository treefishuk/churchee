using Churchee.Common.ValueTypes;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Common.Tests.ValueTypes
{
    public class ImageSimpleTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesToEmptyStrings()
        {
            // Act
            var model = new ImageSimple();

            // Assert
            model.Url.Should().BeEmpty();
            model.TempUrl.Should().BeEmpty();
            model.AltText.Should().BeEmpty();

        }

    }
}