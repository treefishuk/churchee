using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Reviews.Entities;

namespace Churchee.Module.Reviews.Features.Commands
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public CreateReviewCommandHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = new Review(await _currentUser.GetApplicationTenantId())
            {
                Rating = request.Rating,
                ReviewerName = request.ReviewerName,
                Comment = request.Comment,
                SourceName = "Churchee",
                SourceId = "N/A"
            };

            _dataStore.GetRepository<Review>().Create(review);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
