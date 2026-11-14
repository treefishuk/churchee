using Churchee.CQRS.Abstractions;
using System.Collections.Frozen;

namespace Churchee.CQRS.Infrastructure
{
    /// <summary>
    /// Built once at startup, frozen for the lifetime of the application. Singleton.
    /// </summary>
    internal sealed class DispatcherRegistry(
        FrozenDictionary<Type, IRequestHandlerBase> requestWrappers,
        FrozenDictionary<Type, INotificationHandlerBase> notificationWrappers)
    {
        public FrozenDictionary<Type, IRequestHandlerBase> RequestWrappers { get; } = requestWrappers;
        public FrozenDictionary<Type, INotificationHandlerBase> NotificationWrappers { get; } = notificationWrappers;
    }
}
