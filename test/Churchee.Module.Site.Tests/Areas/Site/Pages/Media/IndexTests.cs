using Bunit;
using Churchee.Module.Site.Features.CDN.Queries;
using Churchee.Module.Site.Features.Media.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Moq;
using Index = Churchee.Module.Site.Areas.Site.Pages.Media.Index;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Media
{
    public class IndexTests : BasePageTests
    {

        [Fact]
        public void MediaIndex_HasCorrectName()
        {
            //arrange
            var data = new List<GetMediaFoldersQueryResponseItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetMediaFoldersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);
            MockMediator.Setup(s => s.Send(It.IsAny<GetCDNPathQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(string.Empty);

            SetInitialUrl<Index>();

            //act
            var cut = RenderComponent<Index>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Media");
        }
    }
}
