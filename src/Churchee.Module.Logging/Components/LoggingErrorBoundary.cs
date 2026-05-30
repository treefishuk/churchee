using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Churchee.Module.Logging.Components
{
    public class LoggingErrorBoundary : ErrorBoundary
    {
        [Inject]
        private ILogger<LoggingErrorBoundary> Logger { get; set; }

        [Inject]
        private NavigationManager Nav { get; set; }

        protected override Task OnErrorAsync(Exception exception)
        {
            try
            {
                string route = Nav.ToBaseRelativePath(Nav.Uri);
                Logger.LogError(exception, "UI error at {Route}", route);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Couldn't get Nav Path, original error thrown: {Exception}", exception);
            }

            return base.OnErrorAsync(exception);
        }
    }
}
