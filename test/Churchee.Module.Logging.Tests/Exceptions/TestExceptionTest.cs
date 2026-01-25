using Churchee.Module.Logging.Exceptions;

namespace Churchee.Module.Logging.Tests.Exceptions
{
    public class TestExceptionTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithoutParameters()
        {
            // Act
            var exception = new TestException();

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithMessage()
        {
            // Arrange
            var message = "Test message";

            // Act
            var exception = new TestException(message);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithMessageAndInnerException()
        {
            // Arrange
            var message = "Test message";
            var innerException = new Exception("Inner exception");

            // Act
            var exception = new TestException(message, innerException);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }
    }
}
