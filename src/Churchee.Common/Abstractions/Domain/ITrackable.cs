using System;

namespace Churchee.Common.Abstractions.Entities
{
    public interface ITrackable
    {
        Guid? CreatedById { get; }

        string CreatedByUser { get; }

        DateTime? CreatedDate { get; }

        Guid? ModifiedById { get; }

        string ModifiedByName { get; }

        DateTime? ModifiedDate { get; }
    }
}
