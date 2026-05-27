using Bunit.TestDoubles;
using Churchee.Module.Logging.Components;
using Churchee.Test.Helpers.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Churchee.Module.Logging.Tests.Components
{
    public class LoggingErrorBoundaryTests : BasePageTests
    {
        private class ThrowingComponent : ComponentBase
        {
            protected override void OnInitialized()
            {
                throw new InvalidOperationException("Boom");
            }
        }

        [Fact]
        public async Task LogsRoute_WhenNavWorks()
        {
            // Arrange
            var logger = new Mock<ILogger<LoggingErrorBoundary>>();
            Services.AddSingleton(logger.Object);

            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.NavigateTo("https://localhost/test/path");

            // Act
            var cut = Render<LoggingErrorBoundary>(p =>
                p.AddChildContent<ThrowingComponent>()
            );

            // Assert
            logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }
    }
}