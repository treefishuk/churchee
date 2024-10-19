using Churchee.Module.Hangfire.Filters;
using FluentAssertions;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Churchee.Module.Hangfire.Tests.Filters
{
    public class DashboardAuthorizationFilterTests
    {
        [Fact]
        public void Authorize_ShouldReturnTrue_WhenUserIsAuthenticatedAndIsSysAdmin()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            var userMock = new Mock<ClaimsPrincipal>();
            var identityMock = new Mock<ClaimsIdentity>();

            identityMock.Setup(i => i.IsAuthenticated).Returns(true);
            userMock.Setup(u => u.Identity).Returns(identityMock.Object);
            userMock.Setup(u => u.IsInRole("SysAdmin")).Returns(true);
            httpContextMock.Setup(h => h.User).Returns(userMock.Object);

            var dashboardContextMock = new Mock<DashboardContext>();
            dashboardContextMock.Setup(c => c.GetHttpContext()).Returns(httpContextMock.Object);

            var filter = new DashboardAuthorizationFilter();

            // Act
            var result = filter.Authorize(dashboardContextMock.Object);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Authorize_ShouldReturnFalseAndSetStatusCodeTo404_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            var userMock = new Mock<ClaimsPrincipal>();
            var identityMock = new Mock<ClaimsIdentity>();

            identityMock.Setup(i => i.IsAuthenticated).Returns(false);
            userMock.Setup(u => u.Identity).Returns(identityMock.Object);
            httpContextMock.Setup(h => h.User).Returns(userMock.Object);

            var dashboardContextMock = new Mock<DashboardContext>();
            dashboardContextMock.Setup(c => c.GetHttpContext()).Returns(httpContextMock.Object);

            var filter = new DashboardAuthorizationFilter();

            // Act
            var result = filter.Authorize(dashboardContextMock.Object);

            // Assert
            result.Should().BeFalse();
            httpContextMock.VerifySet(h => h.Response.StatusCode = StatusCodes.Status404NotFound);
        }

        [Fact]
        public void Authorize_ShouldReturnFalseAndSetStatusCodeTo404_WhenUserIsNotSysAdmin()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            var userMock = new Mock<ClaimsPrincipal>();
            var identityMock = new Mock<ClaimsIdentity>();

            identityMock.Setup(i => i.IsAuthenticated).Returns(true);
            userMock.Setup(u => u.Identity).Returns(identityMock.Object);
            userMock.Setup(u => u.IsInRole("SysAdmin")).Returns(false);
            httpContextMock.Setup(h => h.User).Returns(userMock.Object);

            var dashboardContextMock = new Mock<DashboardContext>();
            dashboardContextMock.Setup(c => c.GetHttpContext()).Returns(httpContextMock.Object);

            var filter = new DashboardAuthorizationFilter();

            // Act
            var result = filter.Authorize(dashboardContextMock.Object);

            // Assert
            result.Should().BeFalse();
            httpContextMock.VerifySet(h => h.Response.StatusCode = StatusCodes.Status404NotFound);
        }
    }

}