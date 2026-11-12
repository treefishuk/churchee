using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Reviews.Features.Commands;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Create = Churchee.Module.Reviews.Areas.Reviews.Pages.Create;

namespace Churchee.Module.Reviews.Tests.Areas.Pages.Reviews
{
    public class CreateTests : BasePageTests
    {
        [Fact]
        public void Review_Create_HasCorrectName()
        {
            //arrange
            SetInitialUrl<Create>();

            //act
            var cut = Render<Create>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Create Review");
        }

        [Fact]
        public void Review_Create_HasForm()
        {
            // Arrange
            SetInitialUrl<Create>();

            // Act
            var cut = Render<Create>();

            // Assert
            cut.Find("form").Should().NotBeNull();
        }

        [Fact]
        public void Review_Create_ValidSubmitForm_Navigates_On_Success()
        {
            // Arrange
            var cut = Render<Create>();
            var instance = cut.Instance;

            // Setup InputModel
            instance.InputModel.Rating = 5;
            instance.InputModel.Comment = "Great review!";
            instance.InputModel.ReviewerName = "John Doe";

            // Setup Mediator to return success
            MockMediator.Setup(m => m.Send(It.IsAny<CreateReviewCommand>(), default))
                .ReturnsAsync(new CommandResponse());

            // Act
            var button = cut.Find("#submitFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/reviews");
        }
    }
}
