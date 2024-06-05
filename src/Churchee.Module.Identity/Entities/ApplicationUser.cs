using Churchee.Common.Abstractions.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace Churchee.Module.Identity.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, IEntity
    {

        /// <summary>
        /// For EF
        /// </summary>
        private ApplicationUser()
        {

        }

        public ApplicationUser(Guid applicationTenantId, string userName, string email)
        {
            ApplicationTenantId = applicationTenantId;
            SecurityStamp = Guid.NewGuid().ToString();
            UserName = userName;
            Email = email;
        }

        public Guid ApplicationTenantId { get; private set; }

        public bool Deleted { get; set; }
    }
}
