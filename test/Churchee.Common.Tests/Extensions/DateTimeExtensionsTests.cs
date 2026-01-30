using Churchee.Common.Extensions;

namespace Churchee.Common.Tests.Extensions
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void TruncateToSecondsTest()
        {
            // Arrange
            var dateTime = new DateTime(2024, 1, 1, 1, 1, 34, 22, 22);

            // Act
            var result = dateTime.TruncateToSeconds();

            // Assert
            Assert.Equal(new DateTime(2024, 1, 1, 1, 1, 34, 0, 0), result);
        }

    }
}
