namespace Churchee.CQRS.Abstractions
{
    /// <summary>
    /// Marker for an in-process notification. Multiple handlers per notification are allowed.
    /// </summary>
    public interface INotification;

    public interface INotificationHandler<in TNotification>
        where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken cancellationToken);
    }
}
