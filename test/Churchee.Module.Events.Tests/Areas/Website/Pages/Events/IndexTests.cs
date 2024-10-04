using Bunit;
using Churchee.Common.Abstractions;
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
    }
}
