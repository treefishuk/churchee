using Churchee.Infrastructure.AiTools.Utilities;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Infrastructure.AiTools.Tests.Utilities
{
    public class TestAiToolUtilitiesTests
    {
        [Fact]
        public async Task GenerateAltTextAsync_ShouldReturnGeneratedText()
        {
            // Arrange
            var sut = new TestAiToolUtilities();
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var cancellationToken = CancellationToken.None;

            // Act
            string result = await sut.GenerateAltTextAsync(stream, cancellationToken);

            // Assert
            result.Should().Be("Generated Text");
        }

        [Fact]
        public async Task GenerateAltTextAsync_WhenCancelled_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var sut = new TestAiToolUtilities();
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            Func<Task> act = async () => await sut.GenerateAltTextAsync(stream, cts.Token);

            // Assert
            await act.Should().ThrowAsync<TaskCanceledException>();
        }
    }
}
