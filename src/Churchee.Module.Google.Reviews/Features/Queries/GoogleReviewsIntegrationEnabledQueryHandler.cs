using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.Specifications;
using Churchee.Module.Tokens.Entities;
using MediatR;

namespace Churchee.Module.Google.Reviews.Features.Queries
{
    public class GoogleReviewsIntegrationEnabledQueryHandler : IRequestHandler<GoogleReviewsIntegrationEnabledQuery, bool>
    {
        private readonly IDataStore _dataStore;

        public GoogleReviewsIntegrationEnabledQueryHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<bool> Handle(GoogleReviewsIntegrationEnabledQuery request, CancellationToken cancellationToken)
        {
            bool exists = _dataStore.GetRepository<Token>().ApplySpecification(new HasAccessTokenSpecification()).Any();

            return await Task.FromResult(exists);
        }
    }
}
