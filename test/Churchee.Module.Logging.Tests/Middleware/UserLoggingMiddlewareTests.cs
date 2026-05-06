using Churchee.Common.Abstractions.Auth;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace Churchee.Module.Logging.Tests.Middleware
{
    public class UserLoggingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_CallsNext_WhenNoCurrentUserRegistered()
        {
            // Arrange
            var services = new ServiceCollection().BuildServiceProvider(); // no ICurrentUser registered
            var context = new DefaultHttpContext
            {
                RequestServices = services
            };

            bool nextCalled = false;
            Task next(HttpContext _)
            {
                nextCalled = true;
                return Task.CompletedTask;
            }

            var middleware = new Churchee.Module.Logging.Middleware.UserLoggingMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task InvokeAsync_UsesCurrentUserValues_AndCallsNext()
        {
            // Arrange
            var currentUserMock = new Mock<ICurrentUser>();
            currentUserMock.Setup(x => x.GetUserId()).Returns("user-123");
            currentUserMock.Setup(x => x.GetApplicationTenantName()).ReturnsAsync("tenant-xyz");

            var services = new ServiceCollection()
                .AddSingleton(currentUserMock.Object)
                .BuildServiceProvider();

            var context = new DefaultHttpContext
            {
                RequestServices = services
            };

            // set a username in the HttpContext to exercise username branch
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "bob") }, "test");
            context.User = new ClaimsPrincipal(identity);

            bool nextCalled = false;
            Task next(HttpContext _)
            {
                nextCalled = true;
                return Task.CompletedTask;
            }

            var middleware = new Churchee.Module.Logging.Middleware.UserLoggingMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert - ensure next was called and current user methods were used
            nextCalled.Should().BeTrue();
            currentUserMock.Verify(x => x.GetUserId(), Times.AtLeastOnce);
            currentUserMock.Verify(x => x.GetApplicationTenantName(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task InvokeAsync_IfCurrentUserThrows_ExceptionIsCaughtAndNextStillCalled()
        {
            // Arrange
            var currentUserMock = new Mock<ICurrentUser>();

            // Simulate failure during tenant name lookup
            currentUserMock.Setup(x => x.GetApplicationTenantName()).ThrowsAsync(new InvalidOperationException("boom"));

            var services = new ServiceCollection()
                .AddSingleton(currentUserMock.Object)
                .BuildServiceProvider();

            var context = new DefaultHttpContext
            {
                RequestServices = services
            };

            bool nextCalled = false;
            Task next(HttpContext _)
            {
                nextCalled = true;
                return Task.CompletedTask;
            }

            var middleware = new Churchee.Module.Logging.Middleware.UserLoggingMiddleware(next);

            // Act & Assert - should not throw despite ICurrentUser throwing
            await middleware.InvokeAsync(context);

            nextCalled.Should().BeTrue();
            currentUserMock.Verify(x => x.GetApplicationTenantName(), Times.AtLeastOnce);
        }
    }
}