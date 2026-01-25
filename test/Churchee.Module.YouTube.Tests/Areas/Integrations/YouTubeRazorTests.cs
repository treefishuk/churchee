using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.UI.Components;
using Churchee.Module.Videos.Features.Commands;
using Churchee.Module.YouTube.Features.YouTube.Commands;
using Churchee.Module.YouTube.Features.YouTube.Commands.SyncNow;
using Churchee.Module.YouTube.Features.YouTube.Queries;
using Churchee.Module.YouTube.Spotify.Features.YouTube.Commands;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using System.Text;
using YouTubeRazor = Churchee.Module.YouTube.Areas.Integrations.Pages.YouTube;

namespace Churchee.Module.YouTube.Tests.Areas.Integrations.Pages
{
    public class YouTubeRazorTests : BasePageTests
    {

        private readonly Mock<IDistributedCache> _mockCache;

        public YouTubeRazorTests()
        {
            _mockCache = new Mock<IDistributedCache>();

            Services.AddSingleton<IDistributedCache>(_mockCache.Object);
        }

        [Fact]
        public void Has_Correct_Name()
        {
            //arrange
            var data = new GetYouTubeSettingsResponse("@myawesomechurch", "watch", DateTime.Now.AddDays(-2));

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<YouTubeRazor>();

            //act
            var cut = Render<YouTubeRazor>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("YouTube Integration");
        }

        [Fact]
        public void Has_Form_When_No_Saved_Settings()
        {
            // Arrange
            var data = new GetYouTubeSettingsResponse(null, null, null);

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<YouTubeRazor>();

            // Act
            var cut = Render<YouTubeRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(1);
        }

        [Fact]
        public void Shows_Correct_Markup_When_Configured()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-10);

            var data = new GetYouTubeSettingsResponse("@myawesomechurch", "watch", date);

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<YouTubeRazor>();

            // Act
            var cut = Render<YouTubeRazor>();

            // Assert
            cut.Markup.Should().Contain("YouTube Video Sync is set up!");
            cut.Markup.Should().Contain($"Last Run: {date:dd/MM/yyyy HH:mm}");
        }


        [Fact]
        public void No_Form_When_Saved_Settings()
        {
            // Arrange
            var data = new GetYouTubeSettingsResponse("@myawesomechurch", "watch", DateTime.Now);

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<YouTubeRazor>();

            // Act
            var cut = Render<YouTubeRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(0);
        }

        [Fact]
        public void ValidSubmitForm_Stays_On_Page_On_Success()
        {
            // Arrange
            var data = new GetYouTubeSettingsResponse(null, null, null);

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<YouTubeRazor>();

            var cut = Render<YouTubeRazor>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.ApiKey = "AIza00000000000000000000000000000000000";
            instance.InputModel.NameForContent = "Watch";
            instance.InputModel.ChannelIdentifier = "@myawesomechurch";

            MockMediator.Setup(m => m.Send(It.IsAny<VideosEnabledCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            MockMediator.Setup(m => m.Send(It.IsAny<EnableYouTubeSyncCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/integrations/youtube");
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("YouTube sync configured, syncing will being shortly");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void ValidSubmitForm_EnableVideo_Fails_ShowsMessage()
        {
            // Arrange
            var data = new GetYouTubeSettingsResponse(null, null, null);

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<YouTubeRazor>();

            var cut = Render<YouTubeRazor>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.ApiKey = "AIza00000000000000000000000000000000000";
            instance.InputModel.NameForContent = "Watch";
            instance.InputModel.ChannelIdentifier = "@myawesomechurch";

            // Setup response
            var errorRsponse = new CommandResponse();

            errorRsponse.AddError("Fail", "");

            MockMediator.Setup(m => m.Send(It.IsAny<VideosEnabledCommand>(), default))
                .ReturnsAsync(errorRsponse);

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Failed to enable Videos");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

        [Fact]
        public void ValidSubmitForm_EnableYouTubeSyncCommand_Fails_ShowsMessage()
        {
            // Arrange
            var data = new GetYouTubeSettingsResponse(null, null, null);

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<YouTubeRazor>();

            var cut = Render<YouTubeRazor>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.ApiKey = "AIza00000000000000000000000000000000000";
            instance.InputModel.NameForContent = "Watch";
            instance.InputModel.ChannelIdentifier = "@myawesomechurch";

            // Setup response
            var errorRsponse = new CommandResponse();

            errorRsponse.AddError("Fail", "");

            MockMediator.Setup(m => m.Send(It.IsAny<VideosEnabledCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            MockMediator.Setup(m => m.Send(It.IsAny<EnableYouTubeSyncCommand>(), default))
                .ReturnsAsync(errorRsponse);

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Failed to shedule sync");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }


        [Fact]
        public async Task SyncNow_Shows_Success_Message()
        {
            // Arrange
            var data = new GetYouTubeSettingsResponse("@myawesomechurch", "watch", DateTime.Now);

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var syncResponse = new CommandResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<SyncNowCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(syncResponse);

            SetInitialUrl<YouTubeRazor>();

            var appTenantId = await MockCurrentUser.Object.GetApplicationTenantId();

            _mockCache.Setup(s => s.GetAsync($"YouTubeSyncJobQueued_{appTenantId}", It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes(string.Empty));

            // Act
            var cut = Render<YouTubeRazor>();
            cut.Find("#syncNow").Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Sync Scheduled");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public async Task SyncNow_Already_Running_Shows_Warning_Message()
        {
            // Arrange
            var data = new GetYouTubeSettingsResponse("@myawesomechurch", "watch", DateTime.Now);

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var appTenantId = await MockCurrentUser.Object.GetApplicationTenantId();

            var syncResponse = new CommandResponse();

            syncResponse.AddError("A sync is already queued or running. Please wait 10 mins and try again", string.Empty);

            MockMediator.Setup(s => s.Send(It.IsAny<SyncNowCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(syncResponse);

            _mockCache.Setup(s => s.GetAsync($"YouTubeSyncJobQueued_{appTenantId}", It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes("1"));

            SetInitialUrl<YouTubeRazor>();

            // Act
            var cut = Render<YouTubeRazor>();

            cut.Find("#syncNow").Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("A sync is already queued or running. Please wait 10 mins and try again");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

        [Fact]
        public void DisableSync_Shows_Success_Message()
        {
            // Arrange
            var data = new GetYouTubeSettingsResponse("@myawesomechurch", "watch", DateTime.Now);

            MockMediator.Setup(s => s.Send(It.IsAny<GetYouTubeSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var syncResponse = new CommandResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<DisableYouTubeSyncCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(syncResponse);

            SetInitialUrl<YouTubeRazor>();

            // Act
            var cut = Render<YouTubeRazor>();
            cut.Find("#disableSync").Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Integration Disabled");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

    }
}
