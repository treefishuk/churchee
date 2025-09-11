using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Module.Site.Features.Templates.Queries.GetTemplatesListing;
using Churchee.Module.Site.Features.Templates.Responses;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Moq;
using Index = Churchee.Module.Site.Areas.Site.Pages.Templates.Index;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Templates
{
    public class IndexTests : BasePageTests
    {

        [Fact]
        public void TemplatesIndex_HasCorrectName()
        {
            //arrange
            var data = new DataTableResponse<TemplateListingResponse>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetTemplatesListingQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Index>();

            //act
            var cut = RenderComponent<Index>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Templates");
        }

    }
}
