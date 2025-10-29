using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.Templates.Commands;
using Churchee.Module.UI.Components;
using Churchee.Module.UI.Models;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Create = Churchee.Module.Site.Areas.Site.Pages.Templates.Create;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Templates
{
    public class CreateTests : BasePageTests
    {
        public CreateTests()
        {
            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            Services.AddSingleton(mockAiToolUtilities.Object);
        }

        [Fact]
        public void Template_Create_HasCorrectName()
        {
            //arrange
            var data = new List<DropdownInput>();

            //act
            var cut = RenderComponent<Create>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Create Page Template");
        }

        [Fact]
        public void Template_Create_HasForm()
        {
            // Arrange
            var data = new List<DropdownInput>();

            // Act
            var cut = RenderComponent<Create>();

            // Assert
            cut.Find("form").Should().NotBeNull();
        }

        [Fact]
        public void Template_Create_ValidSubmitForm_Navigates_On_Success()
        {
            // Arrange
            var cut = RenderComponent<Create>();
            var instance = cut.Instance;
            SetInitialUrl<Create>();

            cut.Instance.InputModel.Path = "valid-path";
            cut.Instance.InputModel.Content = "content";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<CreateTemplateCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/templates");
        }

        [Fact]
        public void Template_Create_Cancel_Navigates()
        {
            // Arrange
            SetInitialUrl<Create>();
            var cut = RenderComponent<Create>();

            // Act
            var button = cut.Find("#cancelFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/templates");
        }
    }
}
