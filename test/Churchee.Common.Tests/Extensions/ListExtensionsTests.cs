namespace Churchee.Common.Tests.Extensions
{
    public class ListExtensionsTests
    {
        [Fact]
        public void GetLast_ShouldReturnLastElement()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            var lastElement = list.GetLast();

            // Assert
            Assert.Equal(5, lastElement);
        }
    }
}
