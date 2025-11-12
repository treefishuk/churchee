using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.Pages.Commands.CreatePage;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.Site.Features.PageTypes.Queries;
using Churchee.Module.UI.Components;
using Churchee.Module.UI.Models;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Create = Churchee.Module.Site.Areas.Site.Pages.Pages.Create;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Pages
{
    public class CreateTests : BasePageTests
    {
        [Fact]
        public void Page_Create_HasCorrectName()
        {
            //arrange
            var parentPagesDropdown = new List<DropdownInput>();
            var parentTypesDropdown = new List<SelectListItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetParentPagesDropdownDataQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(parentPagesDropdown);
            MockMediator.Setup(s => s.Send(It.IsAny<GetPagetypesDropdownDataQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(parentTypesDropdown);

            SetInitialUrl<Create>();

            //act
            var cut = Render<Create>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Create Page");
        }

        [Fact]
        public void Page_Create_HasForm()
        {
            // Arrange
            var parentPagesDropdown = new List<DropdownInput>();
            var parentTypesDropdown = new List<SelectListItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetParentPagesDropdownDataQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(parentPagesDropdown);
            MockMediator.Setup(s => s.Send(It.IsAny<GetPagetypesDropdownDataQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(parentTypesDropdown);

            SetInitialUrl<Create>();

            // Act
            var cut = Render<Create>();

            // Assert
            cut.Find("form").Should().NotBeNull();
        }

        [Fact]
        public void Page_Create_ValidSubmitForm_Navigates_On_Success()
        {
            // Arrange
            SetInitialUrl<Create>();

            var cut = Render<Create>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.Title = "Test";
            instance.InputModel.Description = "Desc";
            instance.InputModel.Parent = new DropdownInput { Value = Guid.NewGuid().ToString() };
            instance.InputModel.PageType = new DropdownInput { Value = Guid.NewGuid().ToString() };

            // Setup Mediator to return success 
            MockMediator.Setup(m => m.Send(It.IsAny<CreatePageCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/pages");
        }
    }
}
