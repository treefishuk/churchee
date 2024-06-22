using Churchee.Common.Abstractions.Entities;

namespace Churchee.Module.Events.Entities
{
    public class EventDate : IEntity
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public virtual Event Event { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public bool Deleted { get; set; }
    }
}
