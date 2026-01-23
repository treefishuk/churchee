using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Module.Site.Features.PageTypes.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Moq;
using Index = Churchee.Module.Site.Areas.Site.Pages.PageTypes.Index;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.PageTypes
{
    public class IndexTests : BasePageTests
    {

        [Fact]
        public void PageTypesIndex_HasCorrectName()
        {
            // arrange
            SetInitialUrl<Index>();

            var data = new DataTableResponse<GetPageTypesListingResponse>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetPageTypesListingQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            // act
            var cut = Render<Index>();

            // assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Page Types");
        }
    }
}
