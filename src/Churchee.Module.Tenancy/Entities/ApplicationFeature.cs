using Churchee.Common.Data;

namespace Churchee.Module.Tenancy.Entities
{
    public class ApplicationFeature : Entity
    {
        private ApplicationFeature()
        {
            Name = string.Empty;
        }

        public ApplicationFeature(string name, Guid applicationTenantId) : base(applicationTenantId)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public string Name { get; set; }

    }
}
