using System;

namespace Churchee.Common.Abstractions.Entities
{
    public interface ITenantedEntity
    {
        Guid ApplicationTenantId { get; }
    }
}
