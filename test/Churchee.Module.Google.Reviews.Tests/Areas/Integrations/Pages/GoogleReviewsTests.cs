using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Google.Reviews.Features.Commands;
using Churchee.Module.Google.Reviews.Features.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using GoogleReviewsRazor = Churchee.Module.Google.Reviews.Areas.Integrations.Pages.GoogleReviews;

namespace Churchee.Module.Google.Reviews.Tests.Areas.Integrations.Pages
{
    public class GoogleReviewsTests : BasePageTests
    {
        private readonly Mock<IDistributedCache> _mockCache;

        public GoogleReviewsTests()
        {
            _mockCache = new Mock<IDistributedCache>();

            Services.AddSingleton<IDistributedCache>(_mockCache.Object);

            var mockHttpContextAccessor = GetMockHttpContextAccessor();

            Services.AddSingleton(mockHttpContextAccessor.Object);
        }

        [Fact]
        public void Google_Integration_Has_Correct_Name()
        {
            //arrange
            MockMediator.Setup(s => s.Send(It.IsAny<GoogleReviewsIntegrationEnabledQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<GoogleReviewsRazor>();

            //act
            var cut = Render<GoogleReviewsRazor>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Google Reviews Integration");
        }

        [Fact]
        public void Google_Integration_Has_Form_When_No_Saved_Settings()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<GoogleReviewsIntegrationEnabledQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            SetInitialUrl<GoogleReviewsRazor>();

            // Act
            var cut = Render<GoogleReviewsRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(1);
        }

        [Fact]
        public void Google_Integration_Shows_Correct_Markup_When_Configured()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<GoogleReviewsIntegrationEnabledQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<GoogleReviewsRazor>();

            // Act
            var cut = Render<GoogleReviewsRazor>();

            // Assert
            cut.Markup.Should().Contain("Google Reviews Sync is set up!");
        }


        [Fact]
        public void Google_Integration_No_Form_When_Saved_Settings()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<GoogleReviewsIntegrationEnabledQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<GoogleReviewsRazor>();

            // Act
            var cut = Render<GoogleReviewsRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(0);
        }

        [Fact]
        public void Google_Integration_ValidSubmitForm_Redirects_To_Google_OAuth()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<GoogleReviewsIntegrationEnabledQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            SetInitialUrl<GoogleReviewsRazor>();

            var cut = Render<GoogleReviewsRazor>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.ClientId = "123456";
            instance.InputModel.ClientSecret = "123456";
            instance.InputModel.BusinessProfileId = "123456";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<GetOAuthRedirectQuery>(), default))
                .ReturnsAsync("https://localhost/Google-auth");

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("https://localhost/Google-auth");
        }


        [Fact]
        public void Google_Integration_SyncNow_Success_ShowsSuccess()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<GoogleReviewsIntegrationEnabledQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<GoogleReviewsRazor>();

            // ActivateEventsCommand successful
            MockMediator.Setup(m => m.Send(It.IsAny<SyncNowCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var cut = Render<GoogleReviewsRazor>();

            var button = cut.Find("#syncNow");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Sync Scheduled");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void Google_DisableSync_Success_ShowsSuccess()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<GoogleReviewsIntegrationEnabledQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<GoogleReviewsRazor>();

            MockMediator.Setup(m => m.Send(It.IsAny<DisableGoogleReviewSyncCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var cut = Render<GoogleReviewsRazor>();

            var button = cut.Find("#disableSync");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Integration Disabled");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void Google_DisableSync_Fails_ShowsError()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<GoogleReviewsIntegrationEnabledQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<GoogleReviewsRazor>();

            var reponse = new CommandResponse();

            reponse.AddError("DisableFailed", "");

            MockMediator.Setup(m => m.Send(It.IsAny<DisableGoogleReviewSyncCommand>(), default))
                .ReturnsAsync(reponse);

            // Act
            var cut = Render<GoogleReviewsRazor>();

            var button = cut.Find("#disableSync");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Failed to disable Integration");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

    }
}
