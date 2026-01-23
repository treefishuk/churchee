using Churchee.Module.Identity.Abstractions;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Managers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Churchee.Module.Identity.Tests.Helpers
{
    public static class ChurcheeManagerHelpers
    {
        public static Mock<ChurcheeUserManager> CreateMockChurcheeUserManager()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidatorsMock = new List<IUserValidator<ApplicationUser>> { new Mock<IUserValidator<ApplicationUser>>().Object };
            var passwordValidatorsMock = new List<IPasswordValidator<ApplicationUser>> { new Mock<IPasswordValidator<ApplicationUser>>().Object };
            var keyNormalizerMock = new Mock<ILookupNormalizer>();
            var errorsMock = new Mock<IdentityErrorDescriber>();
            var servicesMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<ChurcheeUserManager>>();

            var userManagerMock = new Mock<ChurcheeUserManager>(
                userStoreMock.Object,
                optionsMock.Object,
                passwordHasherMock.Object,
                userValidatorsMock,
                passwordValidatorsMock,
                keyNormalizerMock.Object,
                errorsMock.Object,
                servicesMock.Object,
                loggerMock.Object);

            return userManagerMock;
        }

        public static Mock<ChurcheeSignInManager> CreateMockChurcheeSignInManager(ChurcheeUserManager churcheeUserManager)
        {
            Mock<IHttpContextAccessor> contextAccessorMock = new();
            Mock<IUserClaimsPrincipalFactory<ApplicationUser>> claimsFactoryMock = new();
            Mock<IOptions<IdentityOptions>> optionsAccessorMock = new();
            Mock<ILogger<ChurcheeSignInManager>> loggerMock = new();
            Mock<IAuthenticationSchemeProvider> schemesMock = new();
            Mock<IUserConfirmation<ApplicationUser>> confirmationMock = new();
            Mock<IIdentitySeed> identitySeedMock = new();

            var userManagerMock = new Mock<ChurcheeSignInManager>(
                churcheeUserManager,
                contextAccessorMock.Object,
                claimsFactoryMock.Object,
                optionsAccessorMock.Object,
                loggerMock.Object,
                schemesMock.Object,
                confirmationMock.Object,
                identitySeedMock.Object
                );

            return userManagerMock;
        }

    }
}
