using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class AllArticlesSpecificationTests
    {
        [Fact]
        public void Specification_Returns_All()
        {
            var a1 = new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "T1", "/t1", "d");
            var a2 = new Article(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "T2", "/t2", "d");

            var list = new[] { a1, a2 };

            var spec = new AllArticlesSpecification();

            var results = spec.Evaluate(list).ToList();

            Assert.Equal(2, results.Count);
        }

    }
}
