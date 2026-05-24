using Churchee.Common.Abstractions.Auth;
using Churchee.CQRS.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;

namespace Churchee.Module.UI.Components
{
    public class BasePage : ComponentBase, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private bool _disposed = false;

        [Inject]
        protected ILogger<BasePage> Logger { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected NotificationService NotificationService { get; set; } = default!;

        [Inject]
        protected ITenantResolver TenantResolver { get; set; } = default!;

        [Inject]
        protected ISender Mediator { get; set; } = default!;

        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
