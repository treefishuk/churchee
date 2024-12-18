using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Module.Podcasts.Features.Queries;
using Churchee.Module.Podcasts.Tests.Areas.Shared.Pages;
using Churchee.Module.Site.Features.CDN.Queries;
using Churchee.Module.UI.Components;
using FluentAssertions;
using Moq;
using Index = Churchee.Module.Podcasts.Areas.Website.Pages.Podcasts.Index;


namespace Churchee.Module.Podcasts.Tests.Areas.Website.Pages
{
    public class IndexRazorTests : BasePageTests
    {
        [Fact]
        public void PodcastsIndex_HasCorrectName()
        {
            // Arrange
            var cdnPath = "https://cdn.example.com/";
            MockMediator.Setup(m => m.Send(It.IsAny<GetCDNPathQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cdnPath);

            var data = new DataTableResponse<GetListingQueryResponseItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetListingQuery>(), default)).ReturnsAsync(data);

            SetInitialUrl<Index>();

            // Act
            var cut = RenderComponent<Index>();

            // Assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Podcasts");

        }


    }
}




