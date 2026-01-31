using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Jotform.Features.Commands;
using Churchee.Module.Jotform.Features.Queries.GetJotformConfigurationStatus;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using JotformRazor = Churchee.Module.Jotform.Areas.Integrations.Pages.Jotform;
namespace Churchee.Module.Jotform.Tests.Areas.Integrations.Pages
{
    public class JotformRazorTests : BasePageTests
    {
        [Fact]
        public async Task Jotform_Integration_Has_Correct_Name()
        {
            // Arrange
            SetInitialUrl<JotformRazor>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetJotformConfigurationStatusQuery>())).ReturnsAsync(new GetJotformConfigurationStatusResponse());

            // Act
            var cut = Render<JotformRazor>();

            // Assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Jotform Integration");
        }


        [Fact]
        public void Jotform_Integration_Has_Form_When_No_Saved_Settings()
        {
            // Arrange
            SetInitialUrl<JotformRazor>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetJotformConfigurationStatusQuery>()))
                .ReturnsAsync(new GetJotformConfigurationStatusResponse { Configured = false });

            // Act
            var cut = Render<JotformRazor>();

            // Assert
            var pageName = cut.FindComponent<PageName>();

            // Assert
            cut.FindAll("form").Count.Should().Be(1);
        }

        [Fact]
        public void Jotform_Integration_Has_No_Form_When_Saved_Settings()
        {
            // Arrange
            SetInitialUrl<JotformRazor>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetJotformConfigurationStatusQuery>()))
                .ReturnsAsync(new GetJotformConfigurationStatusResponse { Configured = true });

            // Act
            var cut = Render<JotformRazor>();

            // Assert
            var pageName = cut.FindComponent<PageName>();

            // Assert
            cut.FindAll("form").Count.Should().Be(0);
        }

        [Fact]
        public void Jotform_Integration_ValidSubmitForm_Stays_On_Page_On_Success()
        {
            // Arrange
            SetInitialUrl<JotformRazor>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetJotformConfigurationStatusQuery>()))
                .ReturnsAsync(new GetJotformConfigurationStatusResponse { Configured = false });

            var cut = Render<JotformRazor>();

            // Setup InputModel
            var instance = cut.Instance;

            instance.InputModel.ApiKey = "1234567890";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<ConfigureJotformCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/integrations/jotform");
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Saved Successfully");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }


        [Fact]
        public void Jotform_Integration_SubmitForm_Fails_Shows_Error()
        {
            // Arrange
            SetInitialUrl<JotformRazor>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetJotformConfigurationStatusQuery>()))
                .ReturnsAsync(new GetJotformConfigurationStatusResponse { Configured = false });

            var cut = Render<JotformRazor>();

            // Setup InputModel
            var instance = cut.Instance;

            instance.InputModel.ApiKey = "1234567890";

            // Setup Mediator to return success
            var response = new CommandResponse();
            response.AddError("Invalid API Key", nameof(instance.InputModel.ApiKey));

            MockMediator.Setup(m => m.Send(It.IsAny<ConfigureJotformCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/integrations/jotform");
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Invalid API Key");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }
    }
}
