using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.Templates.Commands.UpdateTemplateContent;
using Churchee.Module.Site.Features.Templates.Queries.GetTemplateById;
using Churchee.Module.Site.Features.Templates.Responses;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using Edit = Churchee.Module.Site.Areas.Site.Pages.Templates.Edit;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Templates
{
    public class EditTests : BasePageTests
    {
        public EditTests()
        {
            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            Services.AddSingleton(mockAiToolUtilities.Object);
        }

        [Fact]
        public void Template_Edit_HasCorrectName()
        {
            //arrange
            var data = new TemplateDetailResponse { };

            MockMediator.Setup(m => m
            .Send(It.IsAny<GetTemplateByIdQuery>(), default))
                .ReturnsAsync(data);

            //act
            var cut = Render<Edit>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Edit Template");
        }

        [Fact]
        public void Template_Edit_HasForm()
        {
            // Arrange
            var data = new TemplateDetailResponse { };

            MockMediator.Setup(m => m
            .Send(It.IsAny<GetTemplateByIdQuery>(), default))
                .ReturnsAsync(data);

            // Act
            var cut = Render<Edit>();

            // Assert
            cut.Find("form").Should().NotBeNull();
        }

        [Fact]
        public void Template_Edit_ValidSubmitForm_ShowsSuccessMessage()
        {
            // Arrange
            var data = new TemplateDetailResponse { };

            MockMediator.Setup(m => m
            .Send(It.IsAny<GetTemplateByIdQuery>(), default))
                .ReturnsAsync(data);

            var cut = Render<Edit>();
            var instance = cut.Instance;
            instance.InputModel.Id = Guid.NewGuid();
            instance.InputModel.Content = "Content";

            SetInitialUrl<Edit>();

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<UpdateTemplateContentComand>(), default))
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
        public void Template_Edit_Cancel_Navigates()
        {
            // Arrange
            var data = new TemplateDetailResponse { };

            MockMediator.Setup(m => m
            .Send(It.IsAny<GetTemplateByIdQuery>(), default))
                .ReturnsAsync(data);

            SetInitialUrl<Edit>();

            var cut = Render<Edit>();

            // Act
            var button = cut.Find("#cancelFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/templates");
        }
    }
}
