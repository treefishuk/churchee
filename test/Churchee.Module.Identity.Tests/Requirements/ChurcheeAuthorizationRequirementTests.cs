using Churchee.Module.Identity.Requirements;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Churchee.Module.Identity.Tests.Requirements
{
    public class ChurcheeAuthorizationRequirementTests
    {
        [Fact]
        public async Task HandleRequirementAsync_ShouldSucceed_WhenUserIsAuthenticatedAndPathIsNotManagement()
        {
            // Arrange
            var requirement = new ChurcheeAuthorizationRequirement();
            var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "TestUser")], "mock"));
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/somepath";
            var authContext = new AuthorizationHandlerContext([requirement], user, httpContext);

            // Act
            await requirement.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSucceed_WhenUserIsAuthenticatedAndPathIsManagement()
        {
            // Arrange
            var requirement = new ChurcheeAuthorizationRequirement();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "TestUser") }, "mock"));
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/management";
            var authContext = new AuthorizationHandlerContext([requirement], user, httpContext);

            // Act
            await requirement.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldNotSucceed_WhenUserIsAnonymousAndPathIsManagement()
        {
            // Arrange
            var requirement = new ChurcheeAuthorizationRequirement();
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/management";
            var authContext = new AuthorizationHandlerContext([requirement], user, httpContext);

            // Act
            await requirement.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeFalse();
        }
    }

}