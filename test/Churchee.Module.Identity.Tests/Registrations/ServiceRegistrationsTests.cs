using Churchee.Module.Identity.Abstractions;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Managers;
using Churchee.Module.Identity.Registrations;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Churchee.Module.Identity.Tests.Registrations
{
    public class ServiceRegistrationsTests
    {
        [Fact]
        public void ServiceRegistrations_ShouldReturnExpectedServices()
        {
            // Arrange
            var services = new ServiceCollection();

            var serviceRegistrations = new ServiceRegistrations();

            services.AddScoped(s => new Mock<IUserStore<ApplicationUser>>().Object);
            services.AddScoped(s => new Mock<IUserConfirmation<ApplicationUser>>().Object);
            services.AddScoped(s => new Mock<IOptions<IdentityOptions>>().Object);
            services.AddScoped(s => new Mock<IPasswordHasher<ApplicationUser>>().Object);
            services.AddScoped(s => new List<IUserValidator<ApplicationUser>> { new Mock<IUserValidator<ApplicationUser>>().Object });
            services.AddScoped(s => new List<IPasswordValidator<ApplicationUser>> { new Mock<IPasswordValidator<ApplicationUser>>().Object });
            services.AddScoped(s => new Mock<ILookupNormalizer>().Object);
            services.AddScoped(s => new Mock<IAuthenticationSchemeProvider>().Object);
            services.AddScoped(s => new Mock<IdentityErrorDescriber>().Object);
            services.AddScoped(s => new Mock<IServiceProvider>().Object);
            services.AddScoped(s => new Mock<ILogger<ChurcheeUserManager>>().Object);
            services.AddScoped(s => new Mock<DbContext>().Object);
            services.AddScoped(s => new Mock<ILogger<ChurcheeSignInManager>>().Object);
            services.AddScoped(s => new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object);
            services.AddHttpContextAccessor();

            // Act
            serviceRegistrations.Execute(services, null);

            var serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetService<UserManager<ApplicationUser>>().Should().NotBeNull();
            serviceProvider.GetService<ChurcheeUserManager>().Should().NotBeNull();
            serviceProvider.GetService<IIdentitySeed>().Should().NotBeNull();
            serviceProvider.GetService<ISignInManager>().Should().NotBeNull();

        }
    }
}
