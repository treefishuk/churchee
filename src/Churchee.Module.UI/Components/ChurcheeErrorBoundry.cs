using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.UI.Components
{
    public class ChurcheeErrorBoundry : ErrorBoundary
    {
        [Inject]
        protected ILogger<ChurcheeErrorBoundry> Logger { get; set; } = default!;

        protected override Task OnErrorAsync(Exception exception)
        {
            Logger.LogError(exception, "Error boundry hit");

            return Task.CompletedTask;
        }
    }
}
