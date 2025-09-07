using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Managers;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Churchee.Module.Identity.Tests.Managers
{
    public class ChurcheeUserManagerTests
    {
        private readonly Mock<IUserStore<ApplicationUser>> _mockUserStore;
        private readonly Mock<IOptions<IdentityOptions>> _mockOptionsAccessor;
        private readonly Mock<IPasswordHasher<ApplicationUser>> _mockPasswordHasher;
        private readonly List<IUserValidator<ApplicationUser>> _mockUserValidators;
        private readonly List<IPasswordValidator<ApplicationUser>> _mockPasswordValidators;
        private readonly Mock<ILookupNormalizer> _mockKeyNormalizer;
        private readonly Mock<IdentityErrorDescriber> _mockErrors;
        private readonly Mock<IServiceProvider> _mockServices;
        private readonly Mock<ILogger<ChurcheeUserManager>> _mockLogger;

        public ChurcheeUserManagerTests()
        {
            _mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockOptionsAccessor = new Mock<IOptions<IdentityOptions>>();
            _mockPasswordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            _mockUserValidators = new List<IUserValidator<ApplicationUser>> { new Mock<IUserValidator<ApplicationUser>>().Object };
            _mockPasswordValidators = new List<IPasswordValidator<ApplicationUser>> { new Mock<IPasswordValidator<ApplicationUser>>().Object };
            _mockKeyNormalizer = new Mock<ILookupNormalizer>();
            _mockErrors = new Mock<IdentityErrorDescriber>();
            _mockServices = new Mock<IServiceProvider>();
            _mockLogger = new Mock<ILogger<ChurcheeUserManager>>();
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_UserStore_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChurcheeUserManager(
                null,
                _mockOptionsAccessor.Object,
                _mockPasswordHasher.Object,
                _mockUserValidators,
                _mockPasswordValidators,
                _mockKeyNormalizer.Object,
                _mockErrors.Object,
                _mockServices.Object,
                _mockLogger.Object));
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_OptionsAccessor_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChurcheeUserManager(
                _mockUserStore.Object,
                null,
                _mockPasswordHasher.Object,
                _mockUserValidators,
                _mockPasswordValidators,
                _mockKeyNormalizer.Object,
                _mockErrors.Object,
                _mockServices.Object,
                _mockLogger.Object));
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_PasswordHasher_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChurcheeUserManager(
                _mockUserStore.Object,
                _mockOptionsAccessor.Object,
                null,
                _mockUserValidators,
                _mockPasswordValidators,
                _mockKeyNormalizer.Object,
                _mockErrors.Object,
                _mockServices.Object,
                _mockLogger.Object));
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_UserValidators_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChurcheeUserManager(
                _mockUserStore.Object,
                _mockOptionsAccessor.Object,
                _mockPasswordHasher.Object,
                null,
                _mockPasswordValidators,
                _mockKeyNormalizer.Object,
                _mockErrors.Object,
                _mockServices.Object,
                _mockLogger.Object));
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_PasswordValidators_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChurcheeUserManager(
                _mockUserStore.Object,
                _mockOptionsAccessor.Object,
                _mockPasswordHasher.Object,
                _mockUserValidators,
                null,
                _mockKeyNormalizer.Object,
                _mockErrors.Object,
                _mockServices.Object,
                _mockLogger.Object));
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_KeyNormalizer_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChurcheeUserManager(
                _mockUserStore.Object,
                _mockOptionsAccessor.Object,
                _mockPasswordHasher.Object,
                _mockUserValidators,
                _mockPasswordValidators,
                null,
                _mockErrors.Object,
                _mockServices.Object,
                _mockLogger.Object));
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_Errors_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChurcheeUserManager(
                _mockUserStore.Object,
                _mockOptionsAccessor.Object,
                _mockPasswordHasher.Object,
                _mockUserValidators,
                _mockPasswordValidators,
                _mockKeyNormalizer.Object,
                null,
                _mockServices.Object,
                _mockLogger.Object));
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_Services_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChurcheeUserManager(
                _mockUserStore.Object,
                _mockOptionsAccessor.Object,
                _mockPasswordHasher.Object,
                _mockUserValidators,
                _mockPasswordValidators,
                _mockKeyNormalizer.Object,
                _mockErrors.Object,
                null,
                _mockLogger.Object));
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_Logger_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChurcheeUserManager(
                _mockUserStore.Object,
                _mockOptionsAccessor.Object,
                _mockPasswordHasher.Object,
                _mockUserValidators,
                _mockPasswordValidators,
                _mockKeyNormalizer.Object,
                _mockErrors.Object,
                _mockServices.Object,
                null));
        }

        [Fact]
        public void Constructor_Should_Create_Instance_When_All_Dependencies_Are_Provided()
        {
            // Act
            var userManager = new ChurcheeUserManager(
                _mockUserStore.Object,
                _mockOptionsAccessor.Object,
                _mockPasswordHasher.Object,
                _mockUserValidators,
                _mockPasswordValidators,
                _mockKeyNormalizer.Object,
                _mockErrors.Object,
                _mockServices.Object,
                _mockLogger.Object);

            // Assert
            userManager.Should().NotBeNull();
        }
    }
}

