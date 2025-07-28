using Churchee.Common.Data;

namespace Churchee.Module.Tenancy.Entities
{
    public class ApplicationHost : Entity
    {

        private ApplicationHost()
        {
            Host = string.Empty;
            ApplicationTenant = null!;
        }

        public ApplicationHost(string host, Guid applicationTenantId) : base(applicationTenantId)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            Host = host;
            ApplicationTenant = null!;
        }

        public virtual ApplicationTenant ApplicationTenant { get; private set; }

        public string Host { get; private set; }



    }
}
