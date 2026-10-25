using Churchee.Common.Data;

namespace Churchee.Module.Dashboard.Entities
{
    public class AnalyticsDailyDevices : Entity
    {
        public AnalyticsDailyDevices(Guid applicationTenantId) : base(applicationTenantId)
        {
        }

        public DateOnly Date { get; set; }

        public string Device { get; set; }

        public int Views { get; set; }
    }
}
