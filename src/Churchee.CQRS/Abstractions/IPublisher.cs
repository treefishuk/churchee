namespace Churchee.CQRS.Abstractions
{
    public interface IPublisher
    {
        Task Publish<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification;
    }
}
