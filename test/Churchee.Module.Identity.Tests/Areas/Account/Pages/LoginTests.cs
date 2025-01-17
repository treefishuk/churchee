using Churchee.Module.Identity.Abstractions;
using Churchee.Module.Identity.Areas.Account.Pages;

namespace Churchee.Module.Identity.Tests.Areas.Account.Pages
{
    using FluentAssertions;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Threading.Tasks;
    using Xunit;
    using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

    namespace Churchee.Module.Identity.Tests.Areas.Account.Pages
    {
        public class LoginTests
        {
            private readonly Mock<ISignInManager> _signInManagerMock;
            private readonly Mock<ILogger<LoginModel>> _loggerMock;
            private readonly Mock<IUrlHelper> _urlHelperMock;
            private readonly Mock<HttpContext> _httpContextMock;
            private readonly Mock<IServiceProvider> _serviceProviderMock;
            private readonly Mock<IAuthenticationService> _authenticationServiceMock;

            private readonly LoginModel _loginModel;

            public LoginTests()
            {
                _signInManagerMock = new Mock<ISignInManager>();
                _loggerMock = new Mock<ILogger<LoginModel>>();
                _urlHelperMock = new Mock<IUrlHelper>();
                _httpContextMock = new Mock<HttpContext>();
                _serviceProviderMock = new Mock<IServiceProvider>();
                _authenticationServiceMock = new Mock<IAuthenticationService>();

                _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                    .Returns(_authenticationServiceMock.Object);

                _httpContextMock.Setup(c => c.RequestServices)
                    .Returns(_serviceProviderMock.Object);

                _loginModel = new LoginModel(_signInManagerMock.Object, _loggerMock.Object)
                {
                    Url = _urlHelperMock.Object,
                    PageContext = new PageContext
                    {
                        HttpContext = _httpContextMock.Object
                    }
                };
            }

            [Fact]
            public async Task OnGetAsync_Should_ClearExternalCookies()
            {
                // Arrange
                var returnUrl = "~/";
                _loginModel.ErrorMessage = "Some error";

                _authenticationServiceMock.Setup(a => a.SignOutAsync(_httpContextMock.Object, IdentityConstants.ExternalScheme, null))
                    .Returns(Task.CompletedTask);

                // Act
                await _loginModel.OnGetAsync(returnUrl);

                // Assert
                _loginModel.ModelState.IsValid.Should().BeFalse();
                _loginModel?.ModelState[string.Empty]?.Errors.Should().ContainSingle(e => e.ErrorMessage == "Some error");
            }

            [Fact]
            public async Task OnPostAsync_Should_RedirectToReturnUrl_WhenLoginSucceeds()
            {
                // Arrange
                var returnUrl = "~/management";
                _loginModel.Input = new LoginModel.InputModel
                {
                    Email = "test@example.com",
                    Password = "Password123",
                    RememberMe = false
                };

                _urlHelperMock.Setup(u => u.Content(It.IsAny<string>())).Returns((string url) => url);

                _signInManagerMock.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                    .ReturnsAsync(SignInResult.Success);

                // Act
                var result = await _loginModel.OnPostAsync(returnUrl);

                // Assert
                result.Should().BeOfType<LocalRedirectResult>();
                ((LocalRedirectResult)result).Url.Should().Be(returnUrl);
            }

            [Fact]
            public async Task OnPostAsync_Should_ReturnPage_WhenModelStateIsInvalid()
            {
                // Arrange
                _loginModel.ModelState.AddModelError("Email", "The Email field is required.");

                // Act
                var result = await _loginModel.OnPostAsync();

                // Assert
                result.Should().BeOfType<PageResult>();
            }

            [Fact]
            public async Task OnPostAsync_Should_ReturnPage_WhenLoginFails()
            {
                // Arrange
                var returnUrl = "~/management";
                _loginModel.Input = new LoginModel.InputModel
                {
                    Email = "test@example.com",
                    Password = "Password123",
                    RememberMe = false
                };

                _urlHelperMock.Setup(u => u.Content(It.IsAny<string>())).Returns((string url) => url);

                _signInManagerMock.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                    .ReturnsAsync(SignInResult.Failed);

                // Act
                var result = await _loginModel.OnPostAsync(returnUrl);

                // Assert
                result.Should().BeOfType<PageResult>();
                _loginModel?.ModelState[string.Empty]?.Errors.Should().ContainSingle(e => e.ErrorMessage == "Invalid login attempt.");
            }
        }
    }


}