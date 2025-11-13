using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.Abstractions;
using Churchee.Module.Site.Features.Redirects.Queries;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Create = Churchee.Module.Site.Areas.Site.Pages.Redirects.Create;

namespace Churchee.Module.Site.Tests.Areas.Site.Pages.Redirects
{
    public class CreateTests : BasePageTests
    {

        [Fact]
        public void Redirects_Create_HasCorrectName()
        {
            //arrange
            var data = new DataTableResponse<GetListOfRedirectsResponseItem>();

            MockMediator.Setup(s => s.Send(It.IsAny<GetListOfRedirectsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(data);

            SetInitialUrl<Create>();

            //act
            var cut = Render<Create>();

            //assert
            var pageName = cut.FindComponent<PageName>();

            pageName.Instance.Name.Should().Be("Create Redirect");
        }

        [Fact]
        public void Redirects_Create_HasForm()
        {
            // Arrange
            SetInitialUrl<Create>();

            // Act
            var cut = Render<Create>();

            // Assert
            cut.Find("form").Should().NotBeNull();
        }

        [Fact]
        public void Redirects_Create_Cancel_Navigates()
        {
            // Arrange
            SetInitialUrl<Create>();
            var cut = Render<Create>();

            // Act
            var button = cut.Find("#cancelFormBtn");
            button.Click();

            // Assert
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.Uri.Should().Be("http://localhost/management/redirects");
        }
    }
}
