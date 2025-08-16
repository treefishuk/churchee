using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class DevicesSpecification : Specification<PageView>
    {

        public DevicesSpecification(DateTime startDate)
        {
            Query
                .AsNoTracking()
                .Where(x => !string.IsNullOrEmpty(x.Device)
                    && x.ViewedAt > startDate);
        }

    }
}
