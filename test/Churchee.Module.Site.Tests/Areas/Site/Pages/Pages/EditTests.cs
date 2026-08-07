using Bunit;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.Pages.Commands.UpdatePage;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Edit = Churchee.Module.Site.Areas.Site.Pages.Pages.Edit;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Pages
{
    public class EditTests : BasePageTests
    {
        [Fact]
        public void Page_Edit_HasCorrectName()
        {
            //arrange
            var mockImageProcessor = new Mock<IImageProcessor>();
            Services.AddScoped(sp => mockImageProcessor.Object);

            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            Services.AddScoped(sp => mockAiToolUtilities.Object);

            var result = new GetPageDetailsResponse()
            {
                Title = "Edit Page",
                ImageThumbnail = "/_content/images/default-thumbnail.png",
                ContentItems = new List<GetPageDetailsResponseContentItem>(),
                Url = "/edit-page"
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetPageDetailsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            SetInitialUrl<Edit>();

            //act
            var cut = Render<Edit>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Edit Page");
        }


        [Fact]
        public void Page_Edit_HasForm()
        {
            //arrange
            var mockImageProcessor = new Mock<IImageProcessor>();
            Services.AddScoped(sp => mockImageProcessor.Object);

            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            Services.AddScoped(sp => mockAiToolUtilities.Object);

            var result = new GetPageDetailsResponse()
            {
                Title = "Edit Page",
                ImageThumbnail = "/_content/images/default-thumbnail.png",
                ContentItems = new List<GetPageDetailsResponseContentItem>(),
                Url = "/edit-page"
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetPageDetailsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            SetInitialUrl<Edit>();

            //act
            var cut = Render<Edit>();

            //assert
            cut.Find("form").Should().NotBeNull();
        }

        [Fact]
        public void Page_Edit_ValidSubmitForm_Shows_Notification()
        {
            //arrange
            var mockImageProcessor = new Mock<IImageProcessor>();
            Services.AddScoped(sp => mockImageProcessor.Object);

            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            Services.AddScoped(sp => mockAiToolUtilities.Object);

            var result = new GetPageDetailsResponse()
            {
                Title = "Edit Page",
                ImageThumbnail = "/_content/images/default-thumbnail.png",
                ContentItems = new List<GetPageDetailsResponseContentItem>(),
                Url = "/edit-page"
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetPageDetailsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            var goodResult = new CommandResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<UpdatePageCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(goodResult);

            SetInitialUrl<Edit>();

            //act
            var cut = Render<Edit>();
            var button = cut.Find("#submitFormBtn");
            button.Click();

            //assert
            NotificationService.Notifications.Count.Should().Be(1);
        }
    }
}
