using Churchee.Common.Data;

namespace Churchee.Module.Settings.Entities
{
    public class Setting : Entity
    {
        public Setting(Guid id, Guid applicationTenantId, string description, string value) : base(id, applicationTenantId)
        {
            Description = description;
            Value = value;
        }

        public string Description { get; set; }

        public string Value { get; set; }
    }
}
