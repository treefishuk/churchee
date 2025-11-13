using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class CssSpecificationTests
    {
        [Fact]
        public void CssForSpecifiedTenantSpecification_Filters_By_Tenant()
        {
            var tenant = Guid.NewGuid();
            var c1 = new Css(tenant);
            var c2 = new Css(Guid.NewGuid());

            var list = new[] { c1, c2 };

            var spec = new CssForSpecifiedTenantSpecification(tenant);

            var results = spec.Evaluate(list).ToList();

            _ = Assert.Single(results);
            Assert.Equal(tenant, results[0].ApplicationTenantId);
        }
    }
}
