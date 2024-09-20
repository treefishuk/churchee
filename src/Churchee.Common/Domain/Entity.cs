using Churchee.Common.Abstractions.Entities;
using System;

namespace Churchee.Common.Data
{
    public abstract class Entity : IEntity, ITrackable, ITenantedEntity
    {
        public Guid Id { get; protected set; }

        public Guid ApplicationTenantId { get; protected set; }

        public Guid? CreatedById { get; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedByUser { get; set; }

        public Guid? ModifiedById { get; }

        public DateTime? ModifiedDate { get; private set; }

        public string ModifiedByName { get; set; }

        public bool Deleted { get; set; }

        protected Entity() { }

        protected Entity(Guid applicationTenantId)
        {
            Id = Guid.NewGuid();
            ApplicationTenantId = applicationTenantId;
            ModifiedDate = DateTime.Now;
            CreatedDate = DateTime.Now;
        }

        protected Entity(Guid id, Guid applicationTenantId)
        {
            Id = id;
            ApplicationTenantId = applicationTenantId;
            ModifiedDate = DateTime.Now;
            CreatedDate = DateTime.Now;
        }
    }
}
