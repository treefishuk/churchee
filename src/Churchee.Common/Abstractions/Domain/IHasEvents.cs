using Churchee.CQRS.Abstractions;

namespace Churchee.Common.Abstractions.Entities
{
    public interface IHasEvents
    {
        void AddDomainEvent(INotification eventItem);

        void RemoveDomainEvent(INotification eventItem);

        public ICollection<INotification> DomainEvents { get; }

        void ClearDomainEvents();
    }
}
