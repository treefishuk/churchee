using Bunit;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.ValueTypes;
using Churchee.Module.Site.Features.Pages.Commands.UpdatePage;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.Site.Helpers;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;
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

        [Fact]
        public void Page_Edit_ContentTab_Renders_All_Content_Items()
        {
            // Arrange
            var mockImageProcessor = new Mock<IImageProcessor>();
            Services.AddScoped(sp => mockImageProcessor.Object);

            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            Services.AddScoped(sp => mockAiToolUtilities.Object);

            var contentItems = new List<GetPageDetailsResponseContentItem>
            {
                new GetPageDetailsResponseContentItem
                {
                    PageTypeContentId = Guid.NewGuid(),
                    Type = EditorType.RichTextEditor,
                    Title = "Rich Editor",
                    DevName = "richDev",
                    Value = "<p>hello</p>",
                    MaxLength = 0
                },
                new GetPageDetailsResponseContentItem
                {
                    PageTypeContentId = Guid.NewGuid(),
                    Type = EditorType.Number,
                    Title = "Number Field",
                    DevName = "numberDev",
                    Value = "42",
                    MaxLength = 0
                },
                new GetPageDetailsResponseContentItem
                {
                    PageTypeContentId = Guid.NewGuid(),
                    Type = EditorType.MultilineText,
                    Title = "Multi",
                    DevName = "multiDev",
                    Value = "12345",
                    MaxLength = 50
                },
                new GetPageDetailsResponseContentItem
                {
                    PageTypeContentId = Guid.NewGuid(),
                    Type = EditorType.SimpleText,
                    Title = "Simple",
                    DevName = "simpleDev",
                    Value = "abc",
                    MaxLength = 5
                },
                new GetPageDetailsResponseContentItem
                {
                    PageTypeContentId = Guid.NewGuid(),
                    Type = EditorType.Image,
                    Title = "Image",
                    DevName = "imageDev",
                    Value = JsonSerializer.Serialize(new ImageSimple{ Url = "/tmp/image.png" }),
                    MaxLength = 0
                }
            };

            var result = new GetPageDetailsResponse()
            {
                Title = "Edit Page",
                ImageThumbnail = "/_content/images/default-thumbnail.png",
                ContentItems = contentItems,
                Url = "/edit-page"
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetPageDetailsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            SetInitialUrl<Edit>();

            // Act
            var cut = Render<Edit>();

            // Assert - each formfield label should be rendered
            foreach (var item in contentItems)
            {
                cut.Markup.Contains($"{item.Title} ({item.DevName})").Should().BeTrue();
            }
        }

        [Fact]
        public void Page_Edit_ContentTab_Shows_Character_Count_For_MaxLength_Items()
        {
            // Arrange
            var mockImageProcessor = new Mock<IImageProcessor>();
            Services.AddScoped(sp => mockImageProcessor.Object);

            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            Services.AddScoped(sp => mockAiToolUtilities.Object);

            var contentItems = new List<GetPageDetailsResponseContentItem>
            {
                new GetPageDetailsResponseContentItem
                {
                    PageTypeContentId = Guid.NewGuid(),
                    Type = EditorType.MultilineText,
                    Title = "Multi",
                    DevName = "multiDev",
                    Value = "12345",
                    MaxLength = 50
                },
                new GetPageDetailsResponseContentItem
                {
                    PageTypeContentId = Guid.NewGuid(),
                    Type = EditorType.SimpleText,
                    Title = "Simple",
                    DevName = "simpleDev",
                    Value = "abc",
                    MaxLength = 5
                }
            };

            var result = new GetPageDetailsResponse()
            {
                Title = "Edit Page",
                ImageThumbnail = "/_content/images/default-thumbnail.png",
                ContentItems = contentItems,
                Url = "/edit-page"
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetPageDetailsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            SetInitialUrl<Edit>();

            // Act
            var cut = Render<Edit>();

            // Assert - remaining characters text appears and is correctly calculated
            var first = contentItems[0];
            long firstRemaining = (first.MaxLength ?? 0) - (first.Value?.Length ?? 0);
            cut.Markup.Contains($"{firstRemaining} characters remaining").Should().BeTrue();

            var second = contentItems[1];
            long secondRemaining = (second.MaxLength ?? 0) - (second.Value?.Length ?? 0);
            cut.Markup.Contains($"{secondRemaining} characters remaining").Should().BeTrue();
        }


    }
}