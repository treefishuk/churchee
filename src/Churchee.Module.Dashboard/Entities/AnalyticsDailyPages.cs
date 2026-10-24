using Churchee.Common.Data;

namespace Churchee.Module.Dashboard.Entities
{
    public class AnalyticsDailyPages : Entity
    {
        public AnalyticsDailyPages(Guid applicationTenantId) : base(applicationTenantId)
        {
        }

        public DateOnly Date { get; set; }

        public string Url { get; set; }

        public int Views { get; set; }
    }
}
