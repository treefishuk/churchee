using Bunit;
using Churchee.Common.ResponseTypes;
using Churchee.Module.ChurchSuite.Features.Commands.DisableChurchSuiteSync;
using Churchee.Module.ChurchSuite.Features.Commands.EnableChurchSuiteIntegration;
using Churchee.Module.ChurchSuite.Features.Commands.SyncChurchSuiteNow;
using Churchee.Module.ChurchSuite.Features.Queries.ChurchSuiteConfigured;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using ChurchSuiteEventsRazor = Churchee.Module.ChurchSuite.Areas.Integrations.Pages.ChurchSuiteEvents;

namespace Churchee.Module.ChurchSuite.Tests.Areas.Integrations.Pages
{
    public class ChurchSuiteEventsTests : BasePageTests
    {

        private readonly Mock<IDistributedCache> _mockCache;

        public ChurchSuiteEventsTests()
        {
            _mockCache = new Mock<IDistributedCache>();

            Services.AddSingleton<IDistributedCache>(_mockCache.Object);

            var mockHttpContextAccessor = GetMockHttpContextAccessor();

            Services.AddSingleton(mockHttpContextAccessor.Object);
        }

        [Fact]
        public void ChurchSuite_Integration_Has_Correct_Name()
        {
            //arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ChurchSuiteConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<ChurchSuiteEventsRazor>();

            //act
            var cut = Render<ChurchSuiteEventsRazor>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("ChurchSuite Events Integration");
        }

        [Fact]
        public void ChurchSuite_Integration_Has_Form_When_No_Saved_Settings()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ChurchSuiteConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            SetInitialUrl<ChurchSuiteEventsRazor>();

            // Act
            var cut = Render<ChurchSuiteEventsRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(1);
        }

        [Fact]
        public void ChurchSuite_Integration_Shows_Correct_Markup_When_Configured()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ChurchSuiteConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<ChurchSuiteEventsRazor>();

            // Act
            var cut = Render<ChurchSuiteEventsRazor>();

            // Assert
            cut.Markup.Should().Contain("ChurchSuite Events Sync is set up!");
        }


        [Fact]
        public void ChurchSuite_Integration_No_Form_When_Saved_Settings()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ChurchSuiteConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<ChurchSuiteEventsRazor>();

            // Act
            var cut = Render<ChurchSuiteEventsRazor>();

            // Assert
            cut.FindAll("form").Count.Should().Be(0);
        }

        [Fact]
        public void ChurchSuite_Integration_ValidSubmitForm_Success_ShowsSuccess()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ChurchSuiteConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            SetInitialUrl<ChurchSuiteEventsRazor>();

            var cut = Render<ChurchSuiteEventsRazor>();

            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.SubDomain = "demo.churchsuite.com";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<ActivateEventsCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<EnableChurchSuiteIntegrationCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Integration Configured");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void ChurchSuite_Integration_ValidSubmitForm_Error_ShowsError()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ChurchSuiteConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            SetInitialUrl<ChurchSuiteEventsRazor>();

            var cut = Render<ChurchSuiteEventsRazor>();

            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.SubDomain = "demo.churchsuite.com";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<ActivateEventsCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            var errorResponse = new CommandResponse();
            errorResponse.AddError("EnableFailed", "Failed to enable integration");

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<EnableChurchSuiteIntegrationCommand>(), default))
                .ReturnsAsync(errorResponse);

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Failed to configure integration. Please check the subdomain and try again.");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }


        [Fact]
        public void ChurchSuite_Integration_SyncNow_Success_ShowsSuccess()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ChurchSuiteConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<ChurchSuiteEventsRazor>();

            // ActivateEventsCommand successful
            MockMediator.Setup(m => m.Send(It.IsAny<ActivateEventsCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // SyncChurchSuiteEventsToEventsTableCommand successful
            MockMediator.Setup(m => m.Send(It.IsAny<SyncChurchSuiteNowCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var cut = Render<ChurchSuiteEventsRazor>();

            var button = cut.Find("#syncNow");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("ChurchSuite Sync Queued");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void ChurchSuite_DisableSync_Success_ShowsSuccess()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ChurchSuiteConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<ChurchSuiteEventsRazor>();

            MockMediator.Setup(m => m.Send(It.IsAny<DisableChurchSuiteSyncCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var cut = Render<ChurchSuiteEventsRazor>();

            var button = cut.Find("#disableSync");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Integration Disabled");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void ChurchSuite_DisableSync_Fails_ShowsError()
        {
            // Arrange
            MockMediator.Setup(s => s.Send(It.IsAny<ChurchSuiteConfiguredQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            SetInitialUrl<ChurchSuiteEventsRazor>();

            var reponse = new CommandResponse();

            reponse.AddError("DisableFailed", "");

            MockMediator.Setup(m => m.Send(It.IsAny<DisableChurchSuiteSyncCommand>(), default))
                .ReturnsAsync(reponse);

            // Act
            var cut = Render<ChurchSuiteEventsRazor>();

            var button = cut.Find("#disableSync");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Failed to disable Integration");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

    }
}
