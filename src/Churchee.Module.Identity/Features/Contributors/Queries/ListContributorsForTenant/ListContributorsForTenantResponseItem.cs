using System;

namespace Churchee.Module.Identity.Features.Contributors.Queries
{
    public class ListContributorsForTenantResponseItem
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public bool LockedOut { get; set; }

    }

}
