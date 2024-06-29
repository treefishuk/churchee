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

        public ApplicationRole(Guid id, string name) : this(id, name, false)
        {

        }

        public ApplicationRole(Guid id, string name, bool selectable)
        {
            Id = id;
            Name = name;
            NormalizedName = name.ToUpperInvariant();
            Selectable = selectable;
        }

        public bool Deleted { get; set; }

        public bool Selectable { get; set; }

    }
}
