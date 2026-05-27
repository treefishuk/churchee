using Bunit.TestDoubles;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Google.Reviews.Features.Commands;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using GoogleReviewsAuthRazor = Churchee.Module.Google.Reviews.Areas.Integrations.Pages.GoogleReviewsAuth;

namespace Churchee.Module.Google.Reviews.Tests.Areas.Integrations.Pages
{
    public class GoogleReviewsAuthTests : BasePageTests
    {
        [Fact]
        public void OnInitializedAsync_InvokesEnableCommand_AndRedirects()
        {
            // Arrange
            var code = "abc123";
            var state = "stateX";

            // Prepare an HttpContext with scheme/host and track redirects
            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("example.com");

            // Register IHttpContextAccessor that the component will consume
            Services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });

            // Ensure Mediator returns a CommandResponse when the expected command is sent
            MockMediator.Setup(m => m.Send(
                    It.Is<EnableGoogleReviewsIntegrationCommand>(c => c.Code == code && c.Domain == "https://example.com"),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommandResponse())
                .Verifiable();

            // Set the current URI (so SupplyParameterFromQuery is populated)
            var navMan = Services.GetRequiredService<BunitNavigationManager>();
            navMan.NavigateTo($"/management/integrations/google-reviews/auth?code={code}&state={state}", false);

            // Act
            var cut = Render<GoogleReviewsAuthRazor>();

            // Assert - mediator was called with expected command
            MockMediator.Verify();

            // Assert - response was redirected to the expected path
            context.Response.Headers.TryGetValue("Location", out var locationHeader).Should().BeTrue();
            locationHeader.First()!.Should().Be("/management/integrations/google-reviews");
        }
    }
}
