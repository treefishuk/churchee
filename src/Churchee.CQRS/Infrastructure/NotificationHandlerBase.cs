namespace Churchee.CQRS.Infrastructure
{
    internal abstract class NotificationHandlerBase
    {
        public abstract Task Handle(
            object notification,
            IServiceProvider provider,
            CancellationToken cancellationToken);
    }
}
