using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Churchee.Module.Identity.Tests.Extensions
{
    public static class ChurcheeUserManagerMockExtensions
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
    }
}
