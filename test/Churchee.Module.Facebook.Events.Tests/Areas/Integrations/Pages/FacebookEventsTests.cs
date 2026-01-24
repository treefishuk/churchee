using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Facebook.Events.Features.Commands;
using Churchee.Module.Facebook.Events.Features.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using FacebookEventsRazor = Churchee.Module.Facebook.Events.Areas.Integrations.Pages.FacebookEvents;

namespace Churchee.Module.Facebook.Events.Tests.Areas.Integrations.Pages
{
    public class FacebookEventsTests : BasePageTests
    {

        private readonly Mock<IDistributedCache> _mockCache;

        public FacebookEventsTests()
        {
            _mockCache = new Mock<IDistributedCache>();

            Services.AddSingleton<IDistributedCache>(_mockCache.Object);

            var mockHttpContextAccessor = GetMockHttpContextAccessor();

            Services.AddSingleton(mockHttpContextAccessor.Object);
        }

        [Fact]
        public void Facebook_Integration_Has_Correct_Name()
        {
            //arrange
            MockMediator.Setup(s => s.Send(It.IsAny<FacebookConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<FacebookEventsRazor>();

            //act
            var cut = Render<FacebookEventsRazor>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Facebook Events Integration");
        }

        [Fact]
        public void Facebook_Integration_Has_Form_When_No_Saved_Settings()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<FacebookConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            SetInitialUrl<FacebookEventsRazor>();

            // Act
            var cut = Render<FacebookEventsRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(1);
        }

        [Fact]
        public void Facebook_Integration_Shows_Correct_Markup_When_Configured()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<FacebookConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<FacebookEventsRazor>();

            // Act
            var cut = Render<FacebookEventsRazor>();

            // Assert
            cut.Markup.Should().Contain("Facebook Events Sync is set up!");
        }


        [Fact]
        public void Facebook_Integration_No_Form_When_Saved_Settings()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<FacebookConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<FacebookEventsRazor>();

            // Act
            var cut = Render<FacebookEventsRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(0);
        }

        [Fact]
        public void Facebook_Integration_ValidSubmitForm_Redirects_To_Facebook_OAuth()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<FacebookConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            SetInitialUrl<FacebookEventsRazor>();

            var cut = Render<FacebookEventsRazor>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.FacebookPageId = "123456";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<GetAuthUrlQuery>(), default))
                .ReturnsAsync("https://localhost/facebook-auth");

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<ActivateEventsCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("https://localhost/facebook-auth");
        }


        [Fact]
        public void Facebook_Integration_SyncNow_Success_ShowsSuccess()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<FacebookConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<FacebookEventsRazor>();

            // ActivateEventsCommand successful
            MockMediator.Setup(m => m.Send(It.IsAny<ActivateEventsCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // SyncFacebookEventsToEventsTableCommand successful
            MockMediator.Setup(m => m.Send(It.IsAny<SyncFacebookEventsToEventsTableCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var cut = Render<FacebookEventsRazor>();

            var button = cut.Find("#syncNow");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Facebook Sync Queued");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

    }
}
