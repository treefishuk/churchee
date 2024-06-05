using Churchee.Common.Data;

namespace Churchee.Module.Tenancy.Entities
{
    public class ApplicationFeature : Entity
    {
        private ApplicationFeature()
        {

        }  

        public ApplicationFeature(string name, Guid applicationTenantId) : base(applicationTenantId)
        {
            Name = name;    
        }

        public string Name { get; set; }

    }
}
