using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Events.Features.Queries;
using Churchee.Module.Events.Tests.Areas.Shared.Pages;
using Churchee.Module.UI.Components;
using FluentAssertions;
using Moq;
using Radzen;
using Index = Churchee.Module.Events.Areas.Website.Pages.Events.Index;

namespace Churchee.Module.Events.Tests.Areas.Website.Pages.Events
{
    public class IndexTests : BasePageTests
    {
        [Fact]
        public void EventsIndex_HasCorrectName()
        {
            //arrange
            var data = new DataTableResponse<GetListingQueryResponseItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetListingQuery>(), default)).ReturnsAsync(data);

            SetInitialUrl<Index>();

            //act
            var cut = RenderComponent<Index>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Events");
        }

        [Fact]
        public void EventsIndex_DeleteSucceeds_ShowsSuccessMessage()
        {
            //arrange
            SetupGetListingQueryResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<DeleteEventCommand>(), default)).ReturnsAsync(new CommandResponse());

            //act
            var cut = RenderComponent<Index>();

            cut.FindAll(".delete-row")[0].Click();

            //assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Event Deleted");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void EventsIndex_DeleteFails_ShowsErrorMessage()
        {
            //arrange
            SetupGetListingQueryResponse();

            var commandResponse = new CommandResponse();
            commandResponse.AddError("Error", "Thing");

            MockMediator.Setup(s => s.Send(It.IsAny<DeleteEventCommand>(), default)).ReturnsAsync(commandResponse);

            //act
            var cut = RenderComponent<Index>();

            cut.FindAll(".delete-row")[0].Click();

            //assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Error: Could not remove Event");
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

            MockMediator.Setup(s => s.Send(It.IsAny<GetListingQuery>(), default)).ReturnsAsync(data);
        }
    }
}
