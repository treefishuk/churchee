using Churchee.Module.Identity.Abstractions;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Managers;
using Churchee.Module.Identity.Tests.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace Churchee.Module.Identity.Tests.Managers
{


    public class ChurcheeSignInManagerTests
    {
        private readonly Mock<ChurcheeUserManager> _userManagerMock;
        private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
        private readonly Mock<IUserClaimsPrincipalFactory<ApplicationUser>> _claimsFactoryMock;
        private readonly Mock<IOptions<IdentityOptions>> _optionsAccessorMock;
        private readonly Mock<ILogger<ChurcheeSignInManager>> _loggerMock;
        private readonly Mock<IAuthenticationSchemeProvider> _schemesMock;
        private readonly Mock<IUserConfirmation<ApplicationUser>> _confirmationMock;
        private readonly Mock<IIdentitySeed> _identitySeedMock;
        private readonly ChurcheeSignInManager _signInManager;

        public ChurcheeSignInManagerTests()
        {
            _userManagerMock = ChurcheeUserManagerMockExtensions.CreateMockChurcheeUserManager();
            _contextAccessorMock = new Mock<IHttpContextAccessor>();
            _claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
            _loggerMock = new Mock<ILogger<ChurcheeSignInManager>>();
            _schemesMock = new Mock<IAuthenticationSchemeProvider>();
            _confirmationMock = new Mock<IUserConfirmation<ApplicationUser>>();
            _identitySeedMock = new Mock<IIdentitySeed>();

            // Create a mock ClaimsPrincipal
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            // Set up the IUserClaimsPrincipalFactory mock to return the mock ClaimsPrincipal
            _claimsFactoryMock.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(claimsPrincipal);

            // Create a mock HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.User = claimsPrincipal;

            // Mock the SignInAsync method
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(s => s.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(authenticationServiceMock.Object)
                .BuildServiceProvider();

            // Set up the IHttpContextAccessor mock to return the mock HttpContext
            _contextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

            _signInManager = new ChurcheeSignInManager(
                _userManagerMock.Object,
                _contextAccessorMock.Object,
                _claimsFactoryMock.Object,
                _optionsAccessorMock.Object,
                _loggerMock.Object,
                _schemesMock.Object,
                _confirmationMock.Object,
                _identitySeedMock.Object);
        }

        [Fact]
        public async Task PasswordSignInAsync_ShouldCallIdentitySeedCreateAsync()
        {
            // Arrange
            var userName = "testuser";
            var password = "password";
            var isPersistent = false;
            var lockoutOnFailure = false;

            _identitySeedMock.Setup(seed => seed.CreateAsync()).Returns(Task.CompletedTask);

            _userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync(new ApplicationUser(Guid.NewGuid(), userName, "no-one@churchee.com"));
            _userManagerMock.Setup(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), password)).ReturnsAsync(true);

            // Act
            await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

            // Assert
            _identitySeedMock.Verify(seed => seed.CreateAsync(), Times.Once);
        }

        [Fact]
        public async Task PasswordSignInAsync_ShouldReturnSignInResult()
        {
            // Arrange
            var userName = "testuser";
            var password = "password";
            var isPersistent = false;
            var lockoutOnFailure = false;

            _identitySeedMock.Setup(seed => seed.CreateAsync()).Returns(Task.CompletedTask);
            _userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync(new ApplicationUser(Guid.NewGuid(), userName, "no-one@churchee.com"));
            _userManagerMock.Setup(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), password)).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.GetUserIdAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("1");

            // Act
            var result = await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

            // Assert
            result.Should().BeOfType<SignInResult>();
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task PasswordSignInAsync_ShouldReturnFailedResult_WhenUserNotFound()
        {
            // Arrange
            var userName = "nonexistentuser";
            var password = "password";
            var isPersistent = false;
            var lockoutOnFailure = false;

            _identitySeedMock.Setup(seed => seed.CreateAsync()).Returns(Task.CompletedTask);
            _userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

            // Assert
            result.Should().Be(SignInResult.Failed);
        }

        [Fact]
        public async Task PasswordSignInAsync_ShouldReturnFailedResult_WhenPasswordIsIncorrect()
        {
            // Arrange
            var userName = "testuser";
            var password = "wrongpassword";
            var isPersistent = false;
            var lockoutOnFailure = false;

            _identitySeedMock.Setup(seed => seed.CreateAsync()).Returns(Task.CompletedTask);
            _userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync(new ApplicationUser(Guid.NewGuid(), userName, "no-one@churchee.com"));
            _userManagerMock.Setup(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), password)).ReturnsAsync(false);

            // Act
            var result = await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

            // Assert
            result.Should().Be(SignInResult.Failed);
        }
    }

}
