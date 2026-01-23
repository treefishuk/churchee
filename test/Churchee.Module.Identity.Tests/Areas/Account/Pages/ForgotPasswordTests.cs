using Churchee.Common.Abstractions.Utilities;
using Churchee.Module.Identity.Areas.Account.Pages;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Churchee.Module.Identity.Tests.Areas.Account.Pages
{
    public class ForgotPasswordTests
    {
        private static (ForgotPasswordModel sut,
                        Mock<ChurcheeUserManager> userManagerMock,
                        Mock<IEmailService> emailServiceMock,
                        Mock<ILogger<ForgotPasswordModel>> loggerMock,
                        Mock<IUrlHelper> urlHelperMock) CreateSut()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var options = Options.Create(new IdentityOptions());
            var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new List<IUserValidator<ApplicationUser>>();
            var pwdValidators = new List<IPasswordValidator<ApplicationUser>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new IdentityErrorDescriber();
            var services = new ServiceCollection().AddLogging().BuildServiceProvider();
            var userLogger = new Mock<ILogger<ChurcheeUserManager>>();

            var userManagerMock = new Mock<ChurcheeUserManager>(
                store.Object,
                options,
                passwordHasher.Object,
                userValidators,
                pwdValidators,
                keyNormalizer.Object,
                errors,
                services,
                userLogger.Object)
            { CallBase = true };

            var emailServiceMock = new Mock<IEmailService>();
            var loggerMock = new Mock<ILogger<ForgotPasswordModel>>();
            var sut = new ForgotPasswordModel(userManagerMock.Object, emailServiceMock.Object, loggerMock.Object)
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            sut.PageContext.HttpContext.Request.Scheme = "https";

            var urlHelperMock = new Mock<IUrlHelper>();
            sut.Url = urlHelperMock.Object;

            return (sut, userManagerMock, emailServiceMock, loggerMock, urlHelperMock);
        }

        [Fact]
        public async Task OnPostAsync_InvalidModel_ReturnsPage()
        {
            var (sut, userManager, emailService, _, _) = CreateSut();
            sut.ModelState.AddModelError("Email", "Required");
            sut.Input = new ForgotPasswordModel.InputModel { Email = "" };

            var result = await sut.OnPostAsync();

            Assert.IsType<PageResult>(result);
            userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Never);
            emailService.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task OnPostAsync_UserNotFound_RedirectsToConfirmation()
        {
            var (sut, userManager, emailService, _, url) = CreateSut();
            sut.Input = new ForgotPasswordModel.InputModel { Email = "missing@example.com" };

            userManager.Setup(m => m.FindByEmailAsync("missing@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            var result = await sut.OnPostAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./ForgotPasswordConfirmation", redirect.PageName);
            emailService.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task OnPostAsync_EmailNotConfirmed_RedirectsToConfirmation()
        {
            var (sut, userManager, emailService, _, url) = CreateSut();
            sut.Input = new ForgotPasswordModel.InputModel { Email = "user@example.com" };

            var user = new ApplicationUser(Guid.NewGuid(), "user", "user@example.com");
            userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
            userManager.Setup(m => m.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

            var result = await sut.OnPostAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./ForgotPasswordConfirmation", redirect.PageName);
            emailService.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task OnPostAsync_ValidUser_SendsEmail_AndRedirects()
        {
            var (sut, userManager, emailService, _, url) = CreateSut();
            sut.Input = new ForgotPasswordModel.InputModel { Email = "user@example.com" };

            var user = new ApplicationUser(Guid.NewGuid(), "user", "user@example.com");
            userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
            userManager.Setup(m => m.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            userManager.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("reset-token");

            url.SetupGet(h => h.ActionContext)
                .Returns(GetActionContextForPage("/page"));

            url.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns("https://test/reset?code=abc");

            var result = await sut.OnPostAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./ForgotPasswordConfirmation", redirect.PageName);
            emailService.Verify(e =>
                e.SendEmailAsync(user.Email, user.UserName, "Reset your password",
                    It.Is<string>(m => m.Contains("https://test/reset?code=abc")),
                    It.Is<string>(m => m.Contains("https://test/reset?code=abc"))),
                Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_NullCallbackUrl_LogsError_ReturnsPage()
        {
            var (sut, userManager, emailService, logger, url) = CreateSut();
            sut.Input = new ForgotPasswordModel.InputModel { Email = "user@example.com" };

            var user = new ApplicationUser(Guid.NewGuid(), "user", "user@example.com");
            userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
            userManager.Setup(m => m.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            userManager.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("reset-token");

            url.SetupGet(h => h.ActionContext)
                .Returns(GetActionContextForPage("/page"));

            url.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
               .Returns((string?)null);

            var result = await sut.OnPostAsync();

            Assert.IsType<PageResult>(result);
            emailService.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            logger.Verify(l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Callback URL is null")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        private static ActionContext GetActionContextForPage(string page)
        {
            return new()
            {
                ActionDescriptor = new()
                {
                    RouteValues = new Dictionary<string, string?>
                    {
                        { "page", page },
                    }
                },
                RouteData = new()
                {
                    Values =
                    {
                        [ "page" ] = page
                    }
                }
            };
        }
    }
}