using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Module.UI.Components;
using Churchee.Module.Videos.Features.Queries;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Moq;
using Index = Churchee.Module.Videos.Areas.Website.Pages.Videos.Index;


namespace Churchee.Module.Videos.Tests.Areas.Website.Pages
{
    public class IndexRazorTests : BasePageTests
    {
        [Fact]
        public void VideoIndex_HasCorrectName()
        {
            // Arrange
            var data = new DataTableResponse<GetListingQueryResponseItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetListingQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Index>();

            // Act
            var cut = Render<Index>();

            // Assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Videos");

        }


    }
}




