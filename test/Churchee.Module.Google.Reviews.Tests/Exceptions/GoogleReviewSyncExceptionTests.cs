using Churchee.Module.Google.Reviews.Exceptions;

namespace Churchee.Module.Google.Reviews.Tests.Exceptions
{
    public class GoogleReviewSyncExceptionTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithoutParameters()
        {
            // Act
            var exception = new GoogleReviewSyncException();

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithMessage()
        {
            // Arrange
            string message = "Test message";

            // Act
            var exception = new GoogleReviewSyncException(message);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithMessageAndInnerException()
        {
            // Arrange
            string message = "Test message";
            var innerException = new Exception("Inner exception");

            // Act
            var exception = new GoogleReviewSyncException(message, innerException);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }
    }
}
