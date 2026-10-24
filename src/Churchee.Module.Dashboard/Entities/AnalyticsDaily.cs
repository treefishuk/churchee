using Churchee.Common.Data;

namespace Churchee.Module.Dashboard.Entities
{
    public class AnalyticsDaily : Entity
    {
        public AnalyticsDaily(Guid applicationTenantId) : base(applicationTenantId)
        {
        }

        public DateOnly Date { get; set; }
        public int PageViews { get; set; }
        public int UniqueVisitors { get; set; }
        public int ReturningVisitors { get; set; }
    }
}
