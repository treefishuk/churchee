using Bunit;
using Bunit.TestDoubles;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.UI.Tests.Components
{
    public class FormButtonsTests : BasePageTests
    {
        // Render the FormButtons component inside a simple page wrapper to mimic real usage
        private class FormButtonsWrapper : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<FormButtons>(0);
                builder.CloseComponent();
            }
        }

        [Fact]
        public void CancelButton_OnEditPage_NavigatesToCorrectUrl()
        {
            // Arrange
            var id = Guid.NewGuid();

            var navMan = Services.GetRequiredService<BunitNavigationManager>();

            navMan.NavigateTo($"/management/someplace/edit/{id}");

            var cut = Render<FormButtonsWrapper>();

            // Set current URI to a page-like url

            // Act
            cut.Find("#cancelFormBtn").Click();

            // Assert
            Assert.Equal("http://localhost/management/someplace", navMan.Uri);
        }

        [Fact]
        public void CancelButton_OnCreatePage_NavigatesToCorrectUrl()
        {
            // Arrange
            var id = Guid.NewGuid();

            var navMan = Services.GetRequiredService<BunitNavigationManager>();

            navMan.NavigateTo($"/management/someplace/create");

            var cut = Render<FormButtonsWrapper>();

            // Set current URI to a page-like url

            // Act
            cut.Find("#cancelFormBtn").Click();

            // Assert
            Assert.Equal("http://localhost/management/someplace", navMan.Uri);
        }
    }
}
