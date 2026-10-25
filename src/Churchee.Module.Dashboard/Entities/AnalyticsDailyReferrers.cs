using Churchee.Common.Data;

namespace Churchee.Module.Dashboard.Entities
{
    public class AnalyticsDailyReferrers : Entity
    {
        public AnalyticsDailyReferrers(Guid applicationTenantId) : base(applicationTenantId)
        {
        }

        public DateOnly Date { get; set; }

        public string Referrer { get; set; }

        public int Views { get; set; }
    }
}
