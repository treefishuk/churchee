using Churchee.Common.Abstractions.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace Churchee.Common.Data
{
    public abstract class AggregateRoot : Entity, IHasEvents
    {
        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= [];
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        private List<INotification> _domainEvents;

        public List<INotification> DomainEvents => _domainEvents;

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        protected AggregateRoot()
        {
        }

        protected AggregateRoot(Guid applicationTenantId) : base(applicationTenantId)
        {
        }

        protected AggregateRoot(Guid id, Guid applicationTenantId) : base(id, applicationTenantId)
        {
        }
    }
}
