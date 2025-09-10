using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.Redirects.Commands;
using Churchee.Module.Site.Features.Redirects.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Moq;
using Radzen;
using Index = Churchee.Module.Site.Areas.Site.Pages.Redirects.Index;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Redirects
{
    public class IndexTests : BasePageTests
    {

        [Fact]
        public void RedirectsIndex_HasCorrectName()
        {
            //arrange
            var data = new DataTableResponse<GetListOfRedirectsResponseItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetListOfRedirectsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Index>();

            //act
            var cut = RenderComponent<Index>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Redirects");
        }

        [Fact]
        public void RedirectsIndex_DeleteSucceeds_ShowsSuccessMessage()
        {
            // Arrange
            SetupGetListingQueryResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<DeleteRedirectCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CommandResponse());

            // Act
            var cut = RenderComponent<Index>();

            cut.FindAll(".delete-row")[0].Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Redirect Deleted");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void RedirectsIndex_DeleteFails_ShowsErrorMessage()
        {
            // Arrange
            SetupGetListingQueryResponse();

            var commandResponse = new CommandResponse();
            commandResponse.AddError("Error", "Thing");

            MockMediator.Setup(s => s.Send(It.IsAny<DeleteRedirectCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(commandResponse);

            // Act
            var cut = RenderComponent<Index>();

            cut.FindAll(".delete-row")[0].Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Error: Could not remove Redirect");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }


        private void SetupGetListingQueryResponse()
        {
            var data = new DataTableResponse<GetListOfRedirectsResponseItem>
            {
                Data =
                [
                    new() { Id = 1, Path = "/thing" }
                ]
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetListOfRedirectsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);
        }
    }
}
