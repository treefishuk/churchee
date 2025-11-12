using Bunit;
using Churchee.Module.Identity.Components.Layout;
using Churchee.Module.UI.Models;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using Radzen.Blazor;

namespace Churchee.Module.Identity.Tests.Components.Layout
{
    public class ProfileLayoutTests : TestContext
    {
        public ProfileLayoutTests()
        {
            Services.AddSingleton<DialogService>();
            Services.AddSingleton<CurrentPage>();
            Services.AddSingleton<NotificationService>();
            Services.AddSingleton<ContextMenuService>();
            Services.AddSingleton<TooltipService>();
            Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        [Fact]
        public void Renders_Body_Content()
        {
            // Arrange & Act
            var cut = Render<ProfileLayout>(ps => {
                ps.AddMarkupContent(0, "<p id='body-content'>Hello Body</p>");
            });

            // Assert
            cut.Markup.Contains("Hello Body");
            Assert.NotNull(cut.Find("#body-content"));
        }

        [Fact]
        public void ErrorBoundary_Shows_Fallback_On_Exception()
        {
            // Arrange
            var cut = Render<ProfileLayout>(ps => {

                ps.OpenComponent<ThrowingComponent>(0);
                ps.CloseComponent();
            });

            // Act / Assert (fallback content rendered)
            cut.Markup.Should().Contain("Oh dear...");
            cut.Markup.Should().Contain("something has gone wrong");
        }

        [Fact]
        public void SidebarToggle_Toggles_Sidebar_Expanded_State()
        {
            // Arrange
            var cut = Render<ProfileLayout>(ps => {
                ps.AddMarkupContent(0, "<div>Content</div>");
            });

            var sidebar = cut.FindComponent<RadzenSidebar>();
            Assert.True(sidebar.Instance.Expanded);

            // Act
            var toggle = cut.FindComponent<RadzenSidebarToggle>();
            toggle.Find("button").Click(); // RadzenSidebarToggle renders a button internally

            // Re-query (component re-rendered)
            sidebar = cut.FindComponent<RadzenSidebar>();
            Assert.False(sidebar.Instance.Expanded);
        }

        private class ThrowingComponent : ComponentBase
        {
            protected override void OnParametersSet()
            {
                throw new InvalidOperationException("Boom");
            }
        }
    }
}