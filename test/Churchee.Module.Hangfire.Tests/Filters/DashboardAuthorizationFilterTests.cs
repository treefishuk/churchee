using Churchee.Module.Hangfire.Filters;
using FluentAssertions;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Antiforgery;
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
            var storage = new Mock<JobStorage>().Object;
            var options = new DashboardOptions();
            var httpContext = new DefaultHttpContext();

            var antiforgeryMock = new Mock<IAntiforgery>();
            var antiforgeryTokenSet = new AntiforgeryTokenSet("requestToken", "cookieToken", "formFieldName", "headerName");
            antiforgeryMock.Setup(a => a.GetAndStoreTokens(It.IsAny<HttpContext>())).Returns(antiforgeryTokenSet);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IAntiforgery))).Returns(antiforgeryMock.Object);

            var userMock = new Mock<ClaimsPrincipal>();
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(i => i.IsAuthenticated).Returns(true);
            userMock.Setup(u => u.IsInRole("SysAdmin")).Returns(true);
            userMock.Setup(u => u.Identity).Returns(identityMock.Object);

            httpContext.RequestServices = serviceProviderMock.Object;
            httpContext.User = userMock.Object;
            httpContext.Request.Path = "/jobs";

            var filter = new DashboardAuthorizationFilter();

            var context = new AspNetCoreDashboardContext(storage, options, httpContext);

            // Act
            var result = filter.Authorize(context);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Authorize_ShouldReturnFalse_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var storage = new Mock<JobStorage>().Object;
            var options = new DashboardOptions();
            var httpContext = new DefaultHttpContext();

            var antiforgeryMock = new Mock<IAntiforgery>();
            var antiforgeryTokenSet = new AntiforgeryTokenSet("requestToken", "cookieToken", "formFieldName", "headerName");
            antiforgeryMock.Setup(a => a.GetAndStoreTokens(It.IsAny<HttpContext>())).Returns(antiforgeryTokenSet);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IAntiforgery))).Returns(antiforgeryMock.Object);

            var userMock = new Mock<ClaimsPrincipal>();
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(i => i.IsAuthenticated).Returns(false);
            userMock.Setup(u => u.Identity).Returns(identityMock.Object);

            httpContext.RequestServices = serviceProviderMock.Object;
            httpContext.User = userMock.Object;
            httpContext.Request.Path = "/jobs";

            var filter = new DashboardAuthorizationFilter();

            var context = new AspNetCoreDashboardContext(storage, options, httpContext);

            // Act
            var result = filter.Authorize(context);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Authorize_ShouldReturnFalse_WhenUserIsNotSysAdmin()
        {
            // Arrange
            var storage = new Mock<JobStorage>().Object;
            var options = new DashboardOptions();
            var httpContext = new DefaultHttpContext();

            var antiforgeryMock = new Mock<IAntiforgery>();
            var antiforgeryTokenSet = new AntiforgeryTokenSet("requestToken", "cookieToken", "formFieldName", "headerName");
            antiforgeryMock.Setup(a => a.GetAndStoreTokens(It.IsAny<HttpContext>())).Returns(antiforgeryTokenSet);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IAntiforgery))).Returns(antiforgeryMock.Object);

            var userMock = new Mock<ClaimsPrincipal>();
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(i => i.IsAuthenticated).Returns(true);
            userMock.Setup(u => u.IsInRole("SysAdmin")).Returns(false);
            userMock.Setup(u => u.Identity).Returns(identityMock.Object);

            httpContext.RequestServices = serviceProviderMock.Object;
            httpContext.User = userMock.Object;
            httpContext.Request.Path = "/jobs";

            var filter = new DashboardAuthorizationFilter();

            var context = new AspNetCoreDashboardContext(storage, options, httpContext);

            // Act
            var result = filter.Authorize(context);

            // Assert
            result.Should().BeFalse();
        }
    }

}