using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class RedirectsSpecificationTests
    {
        [Fact]
        public void Filters_By_Tenant_And_Search()
        {
            var tenant = Guid.NewGuid();
            var page = new Page(tenant, "T", "/t", "d", Guid.NewGuid(), null, false);
            var r1 = new RedirectUrl("/search", page.Id) { WebContent = page };

            var r2 = new RedirectUrl("/other", page.Id) { WebContent = page };

            var list = new[] { r1, r2 };

            var spec = new RedirectsSpecification("search", tenant);

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
            Assert.Contains(results, r => r.Path.Contains("search"));
        }
    }
}
