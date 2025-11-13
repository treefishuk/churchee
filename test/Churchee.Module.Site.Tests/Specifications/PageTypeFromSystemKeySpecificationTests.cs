using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PageTypeFromSystemKeySpecificationTests
    {
        [Fact]
        public void Constructor_Filters_By_SystemKey_And_Tenant()
        {
            var systemKey = Guid.NewGuid();
            var tenantId = Guid.NewGuid();

            var matching = new PageType(Guid.NewGuid(), systemKey, tenantId, true, "Match", triggerPageTypeCreatedEvent: false);
            var otherTenant = new PageType(Guid.NewGuid(), systemKey, Guid.NewGuid(), true, "OtherTenant", triggerPageTypeCreatedEvent: false);
            var different = new PageType(Guid.NewGuid(), Guid.NewGuid(), tenantId, true, "Different", triggerPageTypeCreatedEvent: false);

            var list = new[] { matching, otherTenant, different };

            var spec = new PageTypeFromSystemKeySpecification(systemKey, tenantId);

            var results = spec.Evaluate(list).ToList();

            _ = Assert.Single(results);
            Assert.Contains(results, r => r.Id == matching.Id);
        }
    }
}
