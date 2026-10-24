using Churchee.Common.Data;

namespace Churchee.Module.Dashboard.Entities
{
    public class AnalyticsHourly : Entity
    {
        public AnalyticsHourly(Guid applicationTenantId) : base(applicationTenantId)
        {
        }

        public DateOnly Date { get; set; }

        public int HourOfDay { get; set; }

        public int Views { get; set; }

    }
}
