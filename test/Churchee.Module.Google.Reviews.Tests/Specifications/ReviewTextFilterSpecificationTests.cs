using Churchee.Module.Google.Reviews.Helpers;
using Churchee.Module.Google.Reviews.Specifications;
using Churchee.Module.Tokens.Entities;

namespace Churchee.Module.Reviews.Tests.Specifications
{
    public class HasAccessTokenSpecificationTests
    {
        [Fact]
        public void Specification_Returns_All()
        {
            var a1 = new Token(Guid.NewGuid(), SettingKeys.GoogleReviewsAccessTokenKey.ToString(), "Value");
            var a2 = new Token(Guid.NewGuid(), Guid.NewGuid().ToString(), "Value");

            var list = new[] { a1, a2 };

            var spec = new HasAccessTokenSpecification();

            var results = spec.Evaluate(list).ToList();

            Assert.Single(results);
        }

    }
}
