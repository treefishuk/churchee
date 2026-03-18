using Ardalis.Specification;
using Churchee.Module.Google.Reviews.Helpers;
using Churchee.Module.Tokens.Entities;

namespace Churchee.Module.Google.Reviews.Specifications
{
    public class HasAccessTokenSpecification : Specification<Token>
    {
        public HasAccessTokenSpecification()
        {
            Query.Where(x => x.Key == SettingKeys.GoogleReviewsAccessTokenKey.ToString()).AsNoTracking();
        }
    }
}
