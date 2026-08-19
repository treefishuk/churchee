using Churchee.Common.ValueTypes;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Common.Tests.ValueTypes
{
    public class ImageSimpleTests
    {
        [Fact]
        public void Constructor_Initializes_Nulls()
        {
            // Act
            var model = new ImageSimple();

            // Assert
            model.Url.Should().BeNull();
            model.TempUrl.Should().BeNull();
            model.AltText.Should().BeNull();

        }

    }
}