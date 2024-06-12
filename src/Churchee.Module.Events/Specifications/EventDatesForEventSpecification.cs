﻿using Ardalis.Specification;
using Churchee.Module.Events.Entities;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Events.Specifications
{
    internal class EventDatesForEventSpecification : Specification<EventDate>
    {
        public EventDatesForEventSpecification(Guid eventId)
        {
            var now = DateTime.Now.AddDays(-1);

            Query.Where(x => x.EventId == eventId && x.Start > now);
        }
     }
}
