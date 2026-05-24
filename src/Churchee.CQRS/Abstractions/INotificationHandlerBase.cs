namespace Churchee.CQRS.Abstractions
{
    internal interface INotificationHandlerBase
    {
        public abstract Task Handle(
            object notification,
            IServiceProvider provider,
            CancellationToken cancellationToken);
    }
}
