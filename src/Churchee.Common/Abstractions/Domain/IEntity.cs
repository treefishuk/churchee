using System;

namespace Churchee.Common.Abstractions.Entities
{
    public interface IEntity
    {
        Guid Id { get; }

        bool Deleted { get; set; }
    }
}
