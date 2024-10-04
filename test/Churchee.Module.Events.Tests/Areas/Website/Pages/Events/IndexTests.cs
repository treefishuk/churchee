using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Events.Features.Queries;
using Churchee.Module.Events.Tests.Areas.Shared.Pages;
using Churchee.Module.UI.Components;
using FluentAssertions;
using Moq;
using Index = Churchee.Module.Events.Areas.Website.Pages.Events.Index;

namespace Churchee.Module.Events.Tests.Areas.Website.Pages.Events
{
    public class IndexTests : BasePageTests
    {
        [Fact]
        public void EventsIndex_HasCorrectname()
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
        public void EventsDelete_DoesntFail()
        {
            //arrange
            var data = new DataTableResponse<GetListingQueryResponseItem>();

            data.Data = new List<GetListingQueryResponseItem>()
            {
                new GetListingQueryResponseItem{ Id = Guid.NewGuid(), Title = "Test" }
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetListingQuery>(), default)).ReturnsAsync(data);
            MockMediator.Setup(s => s.Send(It.IsAny<DeleteEventCommand>(), default)).ReturnsAsync(new CommandResponse());

            //act
            var cut = RenderComponent<Index>();

            cut.FindAll(".delete-row").First().Click();

        }
    }
}
