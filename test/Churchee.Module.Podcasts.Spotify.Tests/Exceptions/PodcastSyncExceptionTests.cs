using Churchee.Module.Podcasts.Spotify.Exceptions;

namespace Churchee.Module.Podcasts.Spotify.Tests.Exceptions
{
    public class PodcastSyncExceptionTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithoutParameters()
        {
            // Act
            var exception = new PodcastSyncException();

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithMessage()
        {
            // Arrange
            var message = "Test message";

            // Act
            var exception = new PodcastSyncException(message);

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
            var exception = new PodcastSyncException(message, innerException);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }
    }
}
