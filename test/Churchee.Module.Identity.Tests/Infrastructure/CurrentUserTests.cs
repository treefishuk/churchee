using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Infrastructure;
using Churchee.Module.Identity.Managers;
using Churchee.Module.Identity.Tests.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Churchee.Module.Identity.Tests.Infrastructure
{
    public class CurrentUserTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ChurcheeUserManager> _mockUserManager;
        private readonly CurrentUser _currentUser;

        public CurrentUserTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockUserManager = ChurcheeUserManagerMockExtensions.CreateMockChurcheeUserManager();
            _currentUser = new CurrentUser(_mockHttpContextAccessor.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task GetApplicationTenantId_Should_Return_TenantId_When_Claim_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var tenantId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("ActiveTenantId", tenantId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(new ApplicationUser(Guid.NewGuid(), "userName", "no-reply@churchee.com"));
            _mockUserManager.Setup(x => x.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(claims);

            // Act
            var result = await _currentUser.GetApplicationTenantId();

            // Assert
            result.Should().Be(tenantId);
        }

        [Fact]
        public async Task GetApplicationTenantId_Should_Return_Empty_Guid_When_Claim_Does_Not_Exist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(new ApplicationUser(Guid.NewGuid(), "userName", "no-reply@churchee.com"));
            _mockUserManager.Setup(x => x.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(claims);

            // Act
            var result = await _currentUser.GetApplicationTenantId();

            // Assert
            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void HasFeature_Should_Return_True_When_Feature_Claim_Exists()
        {
            // Arrange
            var featureName = "TestFeature";
            var claims = new List<Claim>
            {
                new Claim(featureName, "true")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _currentUser.HasFeature(featureName);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasFeature_Should_Return_False_When_Feature_Claim_Does_Not_Exist()
        {
            // Arrange
            var featureName = "TestFeature";
            var claims = new List<Claim>();
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _currentUser.HasFeature(featureName);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasRole_Should_Return_True_When_User_Has_Role()
        {
            // Arrange
            var roleName = "TestRole";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, roleName)
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _currentUser.HasRole(roleName);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasRole_Should_Return_True_When_User_Is_SysAdmin()
        {
            // Arrange
            var roleName = "TestRole";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "SysAdmin")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _currentUser.HasRole(roleName);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasRole_Should_Return_False_When_User_Does_Not_Have_Role()
        {
            // Arrange
            var roleName = "TestRole";
            var claims = new List<Claim>();
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _currentUser.HasRole(roleName);

            // Assert
            result.Should().BeFalse();
        }
    }
}


