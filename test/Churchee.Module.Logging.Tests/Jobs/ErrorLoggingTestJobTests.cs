using Churchee.Module.Logging.Exceptions;
using Churchee.Module.Logging.Jobs;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Churchee.Module.Logging.Tests.Jobs
{
    public class ErrorLoggingTestJobTests
    {

        [Fact]
        public async Task ExecuteAsync_When_Dev_DoesNothing()
        {
            // Arrange
            var mockIHostEnvironment = new Mock<IHostEnvironment>();
            mockIHostEnvironment.Setup(s => s.EnvironmentName).Returns(Environments.Development);
            var cut = new ErrorLoggingTestJob(mockIHostEnvironment.Object);

            // Act
            Task act() => cut.ExecuteAsync(CancellationToken.None);

            var exception = await Record.ExceptionAsync(act);

            // Assert
            exception.Should().BeNull();
        }

        [Fact]
        public async Task ExecuteAsync_When_Production_ThrowsException()
        {
            // Arrange
            var mockIHostEnvironment = new Mock<IHostEnvironment>();
            mockIHostEnvironment.Setup(s => s.EnvironmentName).Returns(Environments.Production);
            var cut = new ErrorLoggingTestJob(mockIHostEnvironment.Object);

            // Act
            Action act = () => cut.ExecuteAsync(CancellationToken.None);

            // Assert
            act.Should().Throw<TestException>("Start up Exception Logging Test");

        }
    }
}
