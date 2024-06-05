using MediatR;
using System;
using System.Collections.Generic;

namespace Churchee.Common.Abstractions.Entities
{
    public interface IHasEvents
    {
        void AddDomainEvent(INotification eventItem);

        void RemoveDomainEvent(INotification eventItem);

        public List<INotification> DomainEvents { get; }

        void ClearDomainEvents();

    }
}
