using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Features.HIBP.Queries;
using Churchee.Module.Identity.Managers;
using Churchee.Module.Identity.Tests.Helpers;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using Create = Churchee.Module.Identity.Areas.Configuration.Pages.Contributors.Create;

namespace Churchee.Module.Identity.Tests.Areas.Configuration.Pages.Contributors
{
    public class CreateTests : BasePageTests
    {
        private readonly Mock<ChurcheeUserManager> _userManager;

        public CreateTests()
        {
            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            Services.AddSingleton(mockAiToolUtilities.Object);
            _userManager = ChurcheeManagerHelpers.CreateMockChurcheeUserManager();
            Services.AddSingleton(_userManager.Object);
        }

        [Fact]
        public void Contributor_Create_HasCorrectName()
        {
            //arrange
            SetInitialUrl<Create>();

            //act
            var cut = Render<Create>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Create Contributor");
        }

        [Fact]
        public void Contributor_Create_HasForm()
        {
            // Arrange
            SetInitialUrl<Create>();

            // Act
            var cut = Render<Create>();

            // Assert
            cut.Find("form").Should().NotBeNull();
        }


        [Fact]
        public void Contributor_Create_Email_Exists_Shows_Error()
        {
            // Arrange
            var cut = Render<Create>();
            SetInitialUrl<Create>();
            var instance = cut.Instance;
            var password = $"{Guid.NewGuid().ToString()}1A!";


            _userManager.Setup(a => a.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser(Guid.NewGuid(), string.Empty, string.Empty));

            // Setup InputModel
            instance.InputModel.Password = password;
            instance.InputModel.ConfirmPassword = password;
            instance.InputModel.Email = "newcontributor@example.com";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<CheckPasswordAgainstHibpCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("E-mail address already registered");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

        [Fact]
        public void Contributor_Create_Password_Powned_Shows_Error()
        {
            // Arrange
            var cut = Render<Create>();
            SetInitialUrl<Create>();
            var instance = cut.Instance;
            var password = $"{Guid.NewGuid().ToString()}1A!";

            // Setup InputModel
            instance.InputModel.Password = password;
            instance.InputModel.ConfirmPassword = password;
            instance.InputModel.Email = "newcontributor@example.com";

            // Setup Mediator to return success
            var response = new CommandResponse();
            response.AddError("Password has been found in data breaches", "Password");

            MockMediator.Setup(m => m.Send(It.IsAny<CheckPasswordAgainstHibpCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Password found in leaked passwords database. Please try something else");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

        [Fact]
        public void Contributor_Create_Fails_Shows_Error()
        {
            // Arrange
            var cut = Render<Create>();
            SetInitialUrl<Create>();
            var instance = cut.Instance;
            var password = $"{Guid.NewGuid().ToString()}1A!";

            var identityResult = IdentityResult.Failed(new IdentityError
            {
                Code = "UserCreationFailed",
                Description = "User creation failed due to unknown error"
            });

            _userManager.Setup(a => a.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(identityResult);

            // Setup InputModel
            instance.InputModel.Password = password;
            instance.InputModel.ConfirmPassword = password;
            instance.InputModel.Email = "newcontributor@example.com";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<CheckPasswordAgainstHibpCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Failed To Create Contributor");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

        [Fact]
        public void Contributor_Create_ValidSubmitForm_Navigates_On_Success()
        {
            // Arrange
            var cut = Render<Create>();
            SetInitialUrl<Create>();
            var instance = cut.Instance;

            var password = $"{Guid.NewGuid().ToString()}1A!";

            // Setup InputModel
            instance.InputModel.Password = password;
            instance.InputModel.ConfirmPassword = password;
            instance.InputModel.Email = "newcontributor@example.com";

            // Setup UserManager to return success
            var identityResult = IdentityResult.Success;

            MockCurrentUser.Setup(a => a.GetApplicationTenantName())
                .ReturnsAsync("ActiveTenant");

            _userManager.Setup(a => a.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(identityResult);

            _userManager.Setup(a => a.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<List<string>>()))
                .ReturnsAsync(identityResult);

            _userManager.Setup(a => a.AddPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                 .ReturnsAsync(identityResult);

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<CheckPasswordAgainstHibpCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/configuration/contributors");
        }

        [Fact]
        public void Contributor_Create_Password_Association_Failure_Shows_Error()
        {
            // Arrange
            var cut = Render<Create>();
            SetInitialUrl<Create>();
            var instance = cut.Instance;

            var password = $"{Guid.NewGuid().ToString()}1A!";

            // Setup InputModel
            instance.InputModel.Password = password;
            instance.InputModel.ConfirmPassword = password;
            instance.InputModel.Email = "newcontributor@example.com";

            // Setup UserManager to return success
            var identityResult = IdentityResult.Success;

            MockCurrentUser.Setup(a => a.GetApplicationTenantName())
                .ReturnsAsync("ActiveTenant");

            _userManager.Setup(a => a.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(identityResult);

            _userManager.Setup(a => a.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<List<string>>()))
                .ReturnsAsync(identityResult);

            var failureIdentityResult = IdentityResult.Failed(new IdentityError
            {
                Code = "UserCreationFailed",
                Description = "User creation failed due to unknown error"
            });

            _userManager.Setup(a => a.AddPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                 .ReturnsAsync(failureIdentityResult);

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<CheckPasswordAgainstHibpCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Failed To assign password to Contributor");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }
    }
}
