using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.Data;

namespace Churchee.Module.Site.Entities
{
    public class MediaFolder : Entity
    {
        public MediaFolder(Guid applicationTenantId, string name) : base(applicationTenantId)
        {
            Name = name;
            Path = $"{Name}/";
        }

        public MediaFolder(Guid applicationTenantId, string name, MediaFolder parent) : base(applicationTenantId)
        {
            Name = name;
            ParentId = parent.Id;
            Path = $"{parent.Path}{Name}/";
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public Guid? ParentId { get; set; }

        public MediaFolder Parent { get; set; }

        public ICollection<MediaFolder> Children { get; set; }
    }
}
