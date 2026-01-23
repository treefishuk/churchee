using Churchee.Infrastructure.AiTools.Settings;
using Churchee.Infrastructure.AiTools.Utilities;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Options;
using Moq;
using System.Reflection;

namespace Churchee.Infrastructure.AiTools.Tests.Utilities
{
    public class AiToolUtilitiesTests
    {
        [Fact]
        public void Constructor_ShouldStoreSettings()
        {
            // Arrange
            var expected = new AzureVisionSettings
            {
                Endpoint = "https://localhost",
                ApiKey = "fake-key"
            };

            var mockOptions = new Mock<IOptions<AzureVisionSettings>>();
            mockOptions.Setup(o => o.Value).Returns(expected);

            // Act
            var sut = new AiToolUtilities(mockOptions.Object);

            // Assert
            var field = typeof(AiToolUtilities).GetField("_settings", BindingFlags.NonPublic | BindingFlags.Instance);
            field.Should().NotBeNull();
            var actual = (AzureVisionSettings)field!.GetValue(sut)!;
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GenerateAltTextAsync_WithNullStream_ShouldThrowArgumentNullException()
        {
            // Arrange
            var settings = new AzureVisionSettings
            {
                Endpoint = "https://localhost",
                ApiKey = "fake-key"
            };

            var mockOptions = new Mock<IOptions<AzureVisionSettings>>();
            mockOptions.Setup(o => o.Value).Returns(settings);

            var sut = new AiToolUtilities(mockOptions.Object);

            // Act
            Func<Task> act = async () => await sut.GenerateAltTextAsync(null!, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GenerateAltTextAsync_WhenCancellationRequested_ShouldThrowTaskCanceledException()
        {
            // Arrange
            var settings = new AzureVisionSettings
            {
                Endpoint = "https://localhost",
                ApiKey = "fake-key"
            };

            var mockOptions = new Mock<IOptions<AzureVisionSettings>>();
            mockOptions.Setup(o => o.Value).Returns(settings);

            var sut = new AiToolUtilities(mockOptions.Object);

            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var cts = new CancellationTokenSource();
            cts.Cancel(); // already canceled

            // Act
            Func<Task> act = async () => await sut.GenerateAltTextAsync(stream, cts.Token);

            // Assert
            await act.Should().ThrowAsync<TaskCanceledException>();
        }
    }
}