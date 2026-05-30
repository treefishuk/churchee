using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class PagesAndArticlesSpecificationTests
    {
        [Fact]
        public void Specification_Returns_Pages_And_Articles()
        {
            var list = new List<WebContent>();

            var a1 = new Page(Guid.NewGuid(), "Apple", "/a", "d", Guid.NewGuid(), null, false);
            var a2 = new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "T2", "/t2", "d");
            var a3 = new AnotherWebContentType(Guid.NewGuid(), "T3", "/t3", "d");

            list.Add(a1);
            list.Add(a2);
            list.Add(a3);

            var spec = new PagesAndArticlesSpecification();

            var results = spec.Evaluate(list).ToList();

            Assert.Equal(2, results.Count);
        }

        private class AnotherWebContentType : WebContent
        {
            public AnotherWebContentType(Guid applicationTenantId, string title, string url, string metadescription) : base(applicationTenantId, title, url, metadescription)
            {
            }

        }
    }
}
