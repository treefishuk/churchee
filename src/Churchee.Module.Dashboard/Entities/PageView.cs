using Churchee.Common.Data;

namespace Churchee.Module.Dashboard.Entities
{
    public class PageView : Entity
    {
        public PageView(Guid applicationTenantId) : base(applicationTenantId)
        {

        }

        public string IpAddress { get; set; }

        public string UserAgent { get; set; }

        public string Url { get; set; }

        public string Device { get; set; }

        public string OS { get; set; }

        public string Browser { get; set; }

        public string Referrer { get; set; }

        public string ReferrerFull { get; set; }

        public DateTime ViewedAt { get; set; }
    }
}
