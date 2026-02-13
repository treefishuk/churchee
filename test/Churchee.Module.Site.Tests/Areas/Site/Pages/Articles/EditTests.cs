using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.Blog.Commands;
using Churchee.Module.Site.Features.Blog.Queries;
using Churchee.Module.UI.Components;
using Churchee.Module.UI.Models;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using Edit = Churchee.Module.Site.Areas.Site.Pages.Articles.Edit;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Articles
{
    public class EditTests : BasePageTests
    {
        public EditTests()
        {
            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            Services.AddSingleton(mockAiToolUtilities.Object);

            var imageProcessorMock = new Mock<IImageProcessor>();
            Services.AddSingleton(imageProcessorMock.Object);

        }

        [Fact]
        public void Articles_Edit_HasCorrectName()
        {
            //arrange
            var data = new GetArticleByIdResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Edit>();

            //act
            var cut = Render<Edit>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Edit Article");
        }

        [Fact]
        public void Article_Edit_HasForm()
        {
            // Arrange
            var data = new GetArticleByIdResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Edit>();

            // Act
            var cut = Render<Edit>();

            // Assert
            cut.Find("form").Should().NotBeNull();
        }

        [Fact]
        public void Article_Edit_DistractionFreeMode_Button_Opens_ContentEditor()
        {
            // Arrange
            var data = new GetArticleByIdResponse()
            {
                IsPublished = true
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            // Act
            var cut = Render<Edit>();

            var button = cut.Find("#distractionFreeModeBtn");
            button.Click();

            // Assert
            DialogService.LastTitle.Should().Be("Update Content");
        }

        [Fact]
        public void Article_Edit_ValidSubmitForm_Navigates_On_Success()
        {
            // Arrange
            var data = new GetArticleByIdResponse();

            MockMediator.Setup(m => m.Send(It.IsAny<UpdateArticleCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(data);

            var cut = Render<Edit>();

            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.Title = "Test";
            instance.InputModel.Title = "Test";
            instance.InputModel.Description = "Desc";
            instance.InputModel.Content = "Content";
            instance.InputModel.Parent = new DropdownInput { Value = Guid.NewGuid().ToString() };

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/articles");
        }



        [Fact]
        public void Article_Edit_If_Published_UnPublish_Visible()
        {
            // Arrange
            var data = new GetArticleByIdResponse()
            {
                IsPublished = true
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            // Act
            var cut = Render<Edit>();

            // Asset
            cut.Find("#unPublishBtn").Should().NotBeNull();
        }

        [Fact]
        public void Article_Edit_If_UnPublished_Publish_Visible()
        {
            // Arrange
            var data = new GetArticleByIdResponse()
            {
                IsPublished = false
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            // Act
            var cut = Render<Edit>();

            // Asset
            cut.Find("#publishBtn").Should().NotBeNull();
        }

        [Fact]
        public void Article_Edit_Successful_Publish()
        {
            // Arrange
            var data = new GetArticleByIdResponse()
            {
                IsPublished = false
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var response = new CommandResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<PublishArticleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var cut = Render<Edit>();

            cut.Find("#publishBtn").Click();

            // Assert
            cut.Find("#unPublishBtn").Should().NotBeNull();

            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Article Published");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void Article_Edit_Publish_Fails()
        {
            // Arrange
            var data = new GetArticleByIdResponse()
            {
                IsPublished = false
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var response = new CommandResponse();
            response.AddError("Publish Failed", "");

            MockMediator.Setup(s => s.Send(It.IsAny<PublishArticleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var cut = Render<Edit>();

            cut.Find("#publishBtn").Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Error: Could not publish Article");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

        [Fact]
        public void Article_Edit_UnPublish_Fails()
        {
            // Arrange
            var data = new GetArticleByIdResponse()
            {
                IsPublished = true
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var response = new CommandResponse();
            response.AddError("Publish Failed", "");

            MockMediator.Setup(s => s.Send(It.IsAny<UnPublishArticleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var cut = Render<Edit>();

            cut.Find("#unPublishBtn").Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Error: Could not remove publication");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Error);
        }

        [Fact]
        public void Article_Edit_UnPublish_Success()
        {
            // Arrange
            var data = new GetArticleByIdResponse()
            {
                IsPublished = true
            };

            MockMediator.Setup(s => s.Send(It.IsAny<GetArticleByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            var response = new CommandResponse();

            MockMediator.Setup(s => s.Send(It.IsAny<UnPublishArticleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var cut = Render<Edit>();

            cut.Find("#unPublishBtn").Click();

            // Assert
            cut.Find("#publishBtn").Should().NotBeNull();

            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Article removed from publication");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }
    }
}
