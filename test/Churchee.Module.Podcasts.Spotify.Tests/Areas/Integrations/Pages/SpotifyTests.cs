using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Podcasts.Features.Commands;
using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands;
using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using System.Text;
using SpotifyRazor = Churchee.Module.Podcasts.Spotify.Areas.Integrations.Pages.Spotify;

namespace Churchee.Module.Podcasts.Spotify.Tests.Areas.Integrations.Pages
{
    public class SpotifyTests : BasePageTests
    {

        private readonly Mock<IDistributedCache> _mockCache;

        public SpotifyTests()
        {
            _mockCache = new Mock<IDistributedCache>();

            Services.AddSingleton<IDistributedCache>(_mockCache.Object);
        }

        [Fact]
        public void Spotify_Integration_Has_Correct_Name()
        {
            //arrange
            var data = new GetPodcastSettingsResponse(string.Empty, string.Empty, null);

            MockMediator.Setup(s => s.Send(It.IsAny<GetPodcastSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<SpotifyRazor>();

            //act
            var cut = Render<SpotifyRazor>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Spotify");
        }

        [Fact]
        public void Spotify_Integration_Has_Form_When_No_Saved_Settings()
        {
            // Arrange
            var data = new GetPodcastSettingsResponse(string.Empty, string.Empty, null);

            MockMediator.Setup(s => s.Send(It.IsAny<GetPodcastSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<SpotifyRazor>();

            // Act
            var cut = Render<SpotifyRazor>();

            // Assert
            cut.MarkupMatches(@"<form>...</form>");
        }

        [Fact]
        public void Spotify_Integration_Shows_Correct_Markup_When_Configured()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-10);

            var data = new GetPodcastSettingsResponse("https://localhost/spotify.xml", "listen", date);

            MockMediator.Setup(s => s.Send(It.IsAny<GetPodcastSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<SpotifyRazor>();

            // Act
            var cut = Render<SpotifyRazor>();

            // Assert
            cut.Markup.Should().Contain("Spotify podcasts is set up!");
            cut.Markup.Should().Contain($"Last Run: {date:dd/MM/yyyy HH:mm}");
        }


        [Fact]
        public void Spotify_Integration_No_Form_When_Saved_Settings()
        {
            // Arrange
            var data = new GetPodcastSettingsResponse("https://localhost/feed.xml", "talks", DateTime.Now.AddDays(-30));

            MockMediator.Setup(s => s.Send(It.IsAny<GetPodcastSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<SpotifyRazor>();

            // Act
            var cut = Render<SpotifyRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(0);
        }

        [Fact]
        public void Spotify_Integration_ValidSubmitForm_Stays_On_Page_On_Success()
        {
            // Arrange
            var data = new GetPodcastSettingsResponse(string.Empty, string.Empty, null);

            MockMediator.Setup(s => s.Send(It.IsAny<GetPodcastSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<SpotifyRazor>();

            var cut = Render<SpotifyRazor>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.SpotifyRSSFeedUrl = "https://localhost/feed.xml ";
            instance.InputModel.NameForContent = "Listen";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<PodcastsEnabledCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<EnableSpotifyPodcastSyncCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/integrations/spotify");
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Spotify Podcasts configured, Syncing will being shortly");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void Spotify_Integration_ValidSubmitForm_Enable_Fails_ShowsMessage()
        {
            // Arrange
            var data = new GetPodcastSettingsResponse(string.Empty, string.Empty, null);

            MockMediator.Setup(s => s.Send(It.IsAny<GetPodcastSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<SpotifyRazor>();

            var cut = Render<SpotifyRazor>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.SpotifyRSSFeedUrl = "https://localhost/feed.xml";
            instance.InputModel.NameForContent = "Listen";

            // Setup response
            var response = new CommandResponse();

            response.AddError("Fail", "");

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<PodcastsEnabledCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Failed to enable Podcasts");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }


        [Fact]
        public void Spotify_Sync_Now_Shows_Success_Message()
        {
            // Arrange
            var data = new GetPodcastSettingsResponse("https://localhost/feed.xml", "talks", DateTime.Now.AddDays(-30));

            MockMediator.Setup(s => s.Send(It.IsAny<GetPodcastSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var syncResponse = new CommandResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<EnableSpotifyPodcastSyncCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(syncResponse);

            SetInitialUrl<SpotifyRazor>();

            // Act
            var cut = Render<SpotifyRazor>();
            cut.Find("#syncNow").Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Sync Scheduled");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public async Task Spotify_Sync_Already_Running_Shows_Warning_Message()
        {
            // Arrange
            var data = new GetPodcastSettingsResponse("https://localhost/feed.xml", "talks", DateTime.Now.AddDays(-30));

            var appTenantId = await MockCurrentUser.Object.GetApplicationTenantId();

            MockMediator.Setup(s => s.Send(It.IsAny<GetPodcastSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var syncResponse = new CommandResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<EnableSpotifyPodcastSyncCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(syncResponse);

            _mockCache.Setup(s => s.GetAsync($"SpotifySyncJobQueued_{appTenantId}", It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes("https://localhost/feed.xml"));

            SetInitialUrl<SpotifyRazor>();

            // Act
            var cut = Render<SpotifyRazor>();

            cut.Find("#syncNow").Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("A sync is already queued or running. Please wait 10 mins and try again");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Warning);
        }

        [Fact]
        public void Spotify_Disable_Sync_Shows_Success_Message()
        {
            // Arrange
            var data = new GetPodcastSettingsResponse("https://localhost/feed.xml", "talks", DateTime.Now.AddDays(-30));

            MockMediator.Setup(s => s.Send(It.IsAny<GetPodcastSettingsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var syncResponse = new CommandResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<DisableSpotifyPodcastSyncCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(syncResponse);

            SetInitialUrl<SpotifyRazor>();

            // Act
            var cut = Render<SpotifyRazor>();
            cut.Find("#disableSync").Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Integration Disabled");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

    }
}
