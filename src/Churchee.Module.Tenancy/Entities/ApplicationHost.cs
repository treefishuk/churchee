using Churchee.Common.Data;

namespace Churchee.Module.Tenancy.Entities
{
    public class ApplicationHost : Entity
    {

        private ApplicationHost()
        {
            Host = string.Empty;
        }

        public ApplicationHost(string host, Guid applicationTenantId) : base(applicationTenantId)
        {
            Host = host;
        }

        public virtual ApplicationTenant ApplicationTenant { get; private set; }

        public string Host { get; private set; }



    }
}
