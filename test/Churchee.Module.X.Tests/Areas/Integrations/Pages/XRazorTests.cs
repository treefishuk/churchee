using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.UI.Components;
using Churchee.Module.X.Features.Tweets.Commands;
using Churchee.Module.X.Features.Tweets.Queries;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using XRazor = Churchee.Module.X.Areas.Integrations.Pages.X;

namespace Churchee.Module.X.Tests.Areas.Integrations.Pages
{
    public class XRazorTests : BasePageTests
    {

        private readonly Mock<IDistributedCache> _mockCache;

        public XRazorTests()
        {
            _mockCache = new Mock<IDistributedCache>();

            Services.AddSingleton<IDistributedCache>(_mockCache.Object);
        }

        [Fact]
        public void X_Integration_Has_Correct_Name()
        {
            //arrange
            var data = new CheckXIntegrationStatusResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<CheckXIntegrationStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<XRazor>();

            //act
            var cut = Render<XRazor>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("X Integration");
        }

        [Fact]
        public void X_Integration_Has_Form_When_No_Saved_Settings()
        {
            // Arrange
            var data = new CheckXIntegrationStatusResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<CheckXIntegrationStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<XRazor>();

            // Act
            var cut = Render<XRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(1);
        }

        [Fact]
        public void X_Integration_Shows_Correct_Markup_When_Configured()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-10);

            var data = new CheckXIntegrationStatusResponse()
            {
                LastRun = date,
                Configured = true
            };

            MockMediator.Setup(s => s.Send(It.IsAny<CheckXIntegrationStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<XRazor>();

            // Act
            var cut = Render<XRazor>();

            // Assert
            cut.Markup.Should().Contain("X Sync is set up!");
            cut.Markup.Should().Contain($"Last Run: {date:dd/MM/yyyy HH:mm}");
        }


        [Fact]
        public void X_Integration_No_Form_When_Saved_Settings()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-10);

            var data = new CheckXIntegrationStatusResponse()
            {
                LastRun = date,
                Configured = true
            };

            MockMediator.Setup(s => s.Send(It.IsAny<CheckXIntegrationStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<XRazor>();

            // Act
            var cut = Render<XRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(0);
        }

        [Fact]
        public void X_Integration_ValidSubmitForm_Stays_On_Page_On_Success()
        {
            // Arrange
            var data = new CheckXIntegrationStatusResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<CheckXIntegrationStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<XRazor>();

            var cut = Render<XRazor>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.AccountName = "@myawesomechurch";
            instance.InputModel.BearerToken = "NotReal";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<EnableTweetsSyncCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/integrations/x");
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("X configured, syncing will being shortly");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void X_Integration_ValidSubmitForm_Enable_Fails_ShowsMessage()
        {
            // Arrange
            var data = new CheckXIntegrationStatusResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<CheckXIntegrationStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<XRazor>();

            var cut = Render<XRazor>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.AccountName = "@myawesomechurch";
            instance.InputModel.BearerToken = "NotReal";

            // Setup response
            var response = new CommandResponse();

            response.AddError("Fail", "");

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<EnableTweetsSyncCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Failed to enable Tweets Sync");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

        [Fact]
        public void X_Disable_Sync_Shows_Success_Message()
        {
            // Arrange
            var data = new CheckXIntegrationStatusResponse()
            {
                LastRun = DateTime.Now.AddDays(-3),
                Configured = true
            };

            MockMediator.Setup(s => s.Send(It.IsAny<CheckXIntegrationStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var response = new CommandResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<DisableTweetsSyncCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

            SetInitialUrl<XRazor>();

            // Act
            var cut = Render<XRazor>();
            cut.Find("#disableSync").Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Integration Disabled");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

    }
}
