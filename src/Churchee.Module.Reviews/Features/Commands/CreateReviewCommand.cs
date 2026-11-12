using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Reviews.Features.Commands
{
    public class CreateReviewCommand : IRequest<CommandResponse>
    {
        public CreateReviewCommand(int rating, string reviewerName, string comment)
        {
            Rating = rating;
            ReviewerName = reviewerName;
            Comment = comment;
        }

        public int Rating { get; set; }

        public string ReviewerName { get; set; }

        public string Comment { get; set; }
    }
}
