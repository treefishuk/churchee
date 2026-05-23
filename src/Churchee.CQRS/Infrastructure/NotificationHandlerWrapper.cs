using Churchee.CQRS.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.CQRS.Infrastructure
{
    internal sealed class NotificationHandlerWrapper<TNotification> : NotificationHandlerBase
        where TNotification : INotification
    {
        public override async Task Handle(
            object notification,
            IServiceProvider provider,
            CancellationToken cancellationToken)
        {
            var typed = (TNotification)notification;
            var handlers = provider.GetServices<INotificationHandler<TNotification>>();

            // Sequential await - simple and predictable. Swap for Task.WhenAll if you want parallel.
            foreach (var handler in handlers)
            {
                await handler.Handle(typed, cancellationToken);
            }
        }
    }
}
