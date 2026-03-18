using Bunit;
using Churchee.Common.Abstractions;
using Churchee.Module.Reviews.Features.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Moq;
using Index = Churchee.Module.Reviews.Areas.Reviews.Pages.Index;

namespace Churchee.Module.Reviews.Tests.Areas.Pages.Reviews
{
    public class IndexTests : BasePageTests
    {

        [Fact]
        public void ReviewsIndex_HasCorrectName()
        {
            //arrange
            var data = new DataTableResponse<GetListingQueryResponseItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetListingQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Index>();

            //act
            var cut = Render<Index>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Reviews");
        }
    }
}
