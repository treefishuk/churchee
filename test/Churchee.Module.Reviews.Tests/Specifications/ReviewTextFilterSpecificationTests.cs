using Churchee.Module.Reviews.Entities;
using Churchee.Module.Reviews.Specifications;

namespace Churchee.Module.Reviews.Tests.Specifications
{
    public class ReviewTextFilterSpecificationTests
    {
        [Fact]
        public void Specification_Returns_All()
        {
            var a1 = new Review(Guid.NewGuid()) { ReviewerName = "Dave" });
            var a2 = new Review(Guid.NewGuid()) { ReviewerName = "Bob" });

            var list = new[] { a1, a2 };

            var spec = new ReviewTextFilterSpecification("Dave");

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
        }

    }
}
