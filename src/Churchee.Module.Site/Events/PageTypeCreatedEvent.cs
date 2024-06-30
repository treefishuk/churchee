﻿using MediatR;

namespace Churchee.Module.Site.Events
{
    public class PageTypeCreatedEvent : INotification
    {
        public PageTypeCreatedEvent(Guid applicationTenantId, string name)
        {
            Name = name;
            ApplicationTenantId = applicationTenantId;
        }

        public string Name { get; set; }

        public Guid ApplicationTenantId { get; set; }

    }
}
