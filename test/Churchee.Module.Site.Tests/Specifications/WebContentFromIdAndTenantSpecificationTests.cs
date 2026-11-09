using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class WebContentFromIdAndTenantSpecificationTests
    {
        [Fact]
        public void Evaluates_Filter_By_Id_And_Tenant()
        {
            var tenantId = Guid.NewGuid();
            var otherTenantId = Guid.NewGuid();

            var match = new Page(tenantId, "Match", "/match", "desc", Guid.NewGuid(), null, false);
            var differentTenant = new Page(otherTenantId, "Wrong Tenant", "/wrong-tenant", "desc", Guid.NewGuid(), null, false);
            var differentId = new Page(tenantId, "Wrong Id", "/wrong-id", "desc", Guid.NewGuid(), null, false);

            var spec = new WebContentFromIdAndTenantSpecification(tenantId, match.Id);
            var results = spec.Evaluate(new WebContent[] { match, differentTenant, differentId }).ToList();

            Assert.Single(results);
            Assert.Contains(results, r => r.Id == match.Id);
        }
    }
}
