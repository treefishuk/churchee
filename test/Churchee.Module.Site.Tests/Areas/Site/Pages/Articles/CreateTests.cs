using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.Blog.Commands;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.UI.Components;
using Churchee.Module.UI.Models;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Create = Churchee.Module.Site.Areas.Site.Pages.Articles.Create;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Articles
{
    public class CreateTests : BasePageTests
    {

        [Fact]
        public void Article_Create_HasCorrectName()
        {
            //arrange
            var data = new List<DropdownInput>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetParentArticlePagesDropdownDataQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Create>();

            //act
            var cut = RenderComponent<Create>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Create Article");
        }

        [Fact]
        public void Article_Create_HasForm()
        {
            // Arrange
            var data = new List<DropdownInput>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetParentArticlePagesDropdownDataQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Create>();

            // Act
            var cut = RenderComponent<Create>();

            // Assert
            cut.Find("form").Should().NotBeNull();
        }

        [Fact]
        public void Article_Create_DistractionFreeMode_Button_Opens_ContentEditor()
        {
            // Arrange
            var cut = RenderComponent<Create>();

            // Act
            var button = cut.Find("#distractionFreeModeBtn");
            button.Click();

            // Assert
            cut.Markup.Contains("Update Content"); // Dialog title
        }

        [Fact]
        public void Article_Create_ValidSubmitForm_Navigates_On_Success()
        {
            // Arrange
            var cut = RenderComponent<Create>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.Title = "Test";
            instance.InputModel.Description = "Desc";
            instance.InputModel.Content = "Content";
            instance.InputModel.Parent = new DropdownInput { Value = Guid.NewGuid().ToString() };

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<CreateArticleCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/articles");
        }
    }
}
