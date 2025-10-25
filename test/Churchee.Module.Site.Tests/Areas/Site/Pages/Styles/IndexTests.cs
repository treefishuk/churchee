using Bunit;
using Churchee.Module.Site.Features.Styles.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Moq;
using Index = Churchee.Module.Site.Areas.Site.Pages.Styles.Index;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Styles
{
    public class IndexTests : BasePageTests
    {

        [Fact]
        public void StylesIndex_HasCorrectName()
        {
            // arrange
            SetInitialUrl<Index>();

            string data = "string";

            MockMediator.Setup(s => s.Send(It.IsAny<GetStylesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            // act
            var cut = RenderComponent<Index>();

            // assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Styles");
        }
    }
}
