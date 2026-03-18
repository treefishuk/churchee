using Ardalis.Specification;
using Churchee.Module.Reviews.Entities;

namespace Churchee.Module.Reviews.Specifications
{
    public class ReviewTextFilterSpecification : Specification<Review>
    {
        public ReviewTextFilterSpecification(string text)
        {
            Query.Where(x => x.ReviewerName.Contains(text));
        }
    }
}
