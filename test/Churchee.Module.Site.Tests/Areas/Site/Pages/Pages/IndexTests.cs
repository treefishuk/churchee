using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.Pages.Commands;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Moq;
using Radzen;
using Index = Churchee.Module.Site.Areas.Site.Pages.Pages.Index;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Pages
{
    public class IndexTests : BasePageTests
    {

        [Fact]
        public void PagesIndex_HasCorrectName()
        {
            //arrange
            var data = new DataTableResponse<GetListingQueryResponseItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetListingQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Index>();

            //act
            var cut = RenderComponent<Index>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Pages");
        }

        [Fact]
        public void PagesIndex_DeleteSucceeds_ShowsSuccessMessage()
        {
            // Arrange
            SetupGetListingQueryResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<DeletePageCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CommandResponse());

            // Act
            var cut = RenderComponent<Index>();

            cut.FindAll(".delete-row")[0].Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Page Deleted");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void PagesIndex_DeleteFails_ShowsErrorMessage()
        {
            // Arrange
            SetupGetListingQueryResponse();

            var commandResponse = new CommandResponse();
            commandResponse.AddError("Error", "Thing");

            MockMediator.Setup(s => s.Send(It.IsAny<DeletePageCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(commandResponse);

            // Act
            var cut = RenderComponent<Index>();

            cut.FindAll(".delete-row")[0].Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Error: Could not remove Page");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }


        private void SetupGetListingQueryResponse()
        {
            var data = new DataTableResponse<GetListingQueryResponseItem>
            {
                Data =
                [
                    new() { Id = Guid.NewGuid(), Title = "Test" }
                ]
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetListingQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);
        }
    }
}
