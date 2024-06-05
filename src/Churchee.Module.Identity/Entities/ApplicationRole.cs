using Churchee.Common.Abstractions.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace Churchee.Module.Identity.Entities
{
    public class ApplicationRole : IdentityRole<Guid>, IEntity
  {
        private ApplicationRole()
        {

        }

        public ApplicationRole(Guid id, string name)
        {
            Id = id;
            Name = name;
            NormalizedName = name.ToUpperInvariant();
        }

        public bool Deleted { get; set; }


    }
}
