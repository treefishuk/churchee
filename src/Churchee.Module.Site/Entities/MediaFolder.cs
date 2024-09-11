using Churchee.Common.Data;

namespace Churchee.Module.Site.Entities
{
    public class MediaFolder : Entity
    {
        public MediaFolder(Guid applicationTenantId, string name, string supportedFileTypes) : base(applicationTenantId)
        {
            Name = name;
            Path = $"{Name}/";
            SupportedFileTypes = supportedFileTypes;
        }

        public MediaFolder(Guid applicationTenantId, string name, MediaFolder parent) : base(applicationTenantId)
        {
            Name = name;
            ParentId = parent.Id;
            Path = $"{parent.Path}{Name}/";
            SupportedFileTypes = parent.SupportedFileTypes;
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public string SupportedFileTypes { get; set; }

        public Guid? ParentId { get; set; }

        public MediaFolder Parent { get; set; }

        public ICollection<MediaFolder> Children { get; set; }
    }
}
