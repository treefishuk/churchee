using Churchee.Module.Identity.Areas.Account.Pages;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Managers;
using Churchee.Module.Identity.Tests.Helpers;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Churchee.Module.Identity.Tests.Areas.Account.Pages
{
    public class LoginWith2faModelTests
    {

        private readonly Mock<ChurcheeSignInManager> _signInManagerMock;
        private readonly Mock<ChurcheeUserManager> _userManagerMock;
        private readonly Mock<ILogger<LoginWith2faModel>> _loggerMock;
        private readonly Mock<IUrlHelper> _urlHelperMock;
        private readonly Mock<HttpContext> _httpContextMock;

        private readonly LoginWith2faModel _model;

        public LoginWith2faModelTests()
        {
            _signInManagerMock = new Mock<ChurcheeSignInManager>();
            _userManagerMock = ChurcheeManagerHelpers.CreateMockChurcheeUserManager();
            _signInManagerMock = ChurcheeManagerHelpers.CreateMockChurcheeSignInManager(_userManagerMock.Object);
            _loggerMock = new Mock<ILogger<LoginWith2faModel>>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _httpContextMock = new Mock<HttpContext>();
            _model = new LoginWith2faModel(_signInManagerMock.Object, _userManagerMock.Object, _loggerMock.Object)
            {
                Url = _urlHelperMock.Object,
                PageContext = new PageContext
                {
                    HttpContext = _httpContextMock.Object
                }
            };
        }

        [Fact]
        public async Task OnGetAsync_UserNotFound_Throws()
        {
            // Arrange
            _signInManagerMock.Setup(m => m.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync((ApplicationUser?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _model.OnGetAsync(false, null));
        }

        [Fact]
        public async Task OnGetAsync_SetsPropertiesAndReturnsPage()
        {
            // Arrange
            var fakeUser = new ApplicationUser(Guid.NewGuid(), string.Empty, string.Empty);
            _signInManagerMock.Setup(m => m.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync(fakeUser);

            // Act
            var result = await _model.OnGetAsync(true, "/return");

            // Assert
            Assert.Equal("/return", _model.ReturnUrl);
            Assert.True(_model.RememberMe);
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_InvalidModelState_ReturnsPage()
        {
            // Arrange
            _model.ModelState.AddModelError("Input.TwoFactorCode", "Required");

            // Act
            var result = await _model.OnPostAsync(false, null);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_UserNotFound_Throws()
        {
            // Arrange
            _model.Input = new LoginWith2faModel.InputModel { TwoFactorCode = "123456", RememberMachine = false };
            _signInManagerMock.Setup(m => m.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync((ApplicationUser?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _model.OnPostAsync(false, null));
        }

        [Fact]
        public async Task OnPostAsync_Successful2fa_RedirectsToReturnUrl()
        {
            // Arrange
            var fakeUser = new ApplicationUser(Guid.NewGuid(), string.Empty, string.Empty);
            _model.Input = new LoginWith2faModel.InputModel { TwoFactorCode = "123456", RememberMachine = false };
            _signInManagerMock.Setup(m => m.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync(fakeUser);
            _signInManagerMock.Setup(m => m.TwoFactorAuthenticatorSignInAsync(It.IsAny<string>(), false, false))
                .ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(m => m.GetUserIdAsync(fakeUser)).ReturnsAsync("userId");

            // Act
            var result = await _model.OnPostAsync(false, "/custom");

            // Assert
            var redirect = Assert.IsType<LocalRedirectResult>(result);
            Assert.Equal("/custom", redirect.Url);
        }

        [Fact]
        public async Task OnPostAsync_LockedOut_RedirectsToLockout()
        {
            // Arrange
            var fakeUser = new ApplicationUser(Guid.NewGuid(), string.Empty, string.Empty);
            _model.Input = new LoginWith2faModel.InputModel { TwoFactorCode = "123456", RememberMachine = false };
            _signInManagerMock.Setup(m => m.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync(fakeUser);
            _signInManagerMock.Setup(m => m.TwoFactorAuthenticatorSignInAsync(It.IsAny<string>(), false, false))
                .ReturnsAsync(SignInResult.LockedOut);
            _userManagerMock.Setup(m => m.GetUserIdAsync(fakeUser)).ReturnsAsync("userId");

            // Act
            var result = await _model.OnPostAsync(false, null);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Lockout", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_InvalidCode_ReturnsPageWithModelError()
        {
            // Arrange
            var fakeUser = new ApplicationUser(Guid.NewGuid(), string.Empty, string.Empty);
            _model.Input = new LoginWith2faModel.InputModel { TwoFactorCode = "badcode", RememberMachine = false };
            _signInManagerMock.Setup(m => m.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync(fakeUser);
            _signInManagerMock.Setup(m => m.TwoFactorAuthenticatorSignInAsync(It.IsAny<string>(), false, false))
                .ReturnsAsync(SignInResult.Failed);
            _userManagerMock.Setup(m => m.GetUserIdAsync(fakeUser)).ReturnsAsync("userId");

            // Act
            var result = await _model.OnPostAsync(false, null);

            // Assert
            Assert.IsType<PageResult>(result);

            _model.ModelState[string.Empty]?.Errors.First().ErrorMessage.Should().Be("Invalid authenticator code.");
        }
    }
}