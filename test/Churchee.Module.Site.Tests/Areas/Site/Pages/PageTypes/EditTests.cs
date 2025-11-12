using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.PageTypes.Commands.UpdatePageTypeContent;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using Edit = Churchee.Module.Site.Areas.Site.Pages.PageTypes.Edit;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.PageTypes
{
    public class EditTests : BasePageTests
    {

        [Fact]
        public void PageType_Edit_HasCorrectName()
        {
            //arrange
            SetInitialUrl<Edit>();

            //act
            var cut = Render<Edit>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Edit Page Type");
        }

        [Fact]
        public void PageType_Edit_HasForm()
        {
            // Arrange
            SetInitialUrl<Edit>();

            // Act
            var cut = Render<Edit>();

            // Assert
            cut.Find("form").Should().NotBeNull();
        }

        [Fact]
        public void PageType_Edit_ValidSubmitForm_ShowsSuccessMessage()
        {
            // Arrange
            var cut = Render<Edit>();

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<UpdatePageTypeContentCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            NotificationService.Notifications.Count.Should().Be(1);
            NotificationService.Notifications.First().Summary.Should().Be("Saved Successfully");
            NotificationService.Notifications.First().Severity.Should().Be(NotificationSeverity.Success);
        }

        [Fact]
        public void PageType_Edit_Cancel_Navigates()
        {
            // Arrange
            var cut = Render<Edit>();

            // Act
            var button = cut.Find("#cancelFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/pagetypes");
        }
    }
}
