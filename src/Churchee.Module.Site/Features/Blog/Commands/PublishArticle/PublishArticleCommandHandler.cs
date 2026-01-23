using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class PublishArticleCommandHandler : IRequestHandler<PublishArticleCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;

        public PublishArticleCommandHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(PublishArticleCommand request, CancellationToken cancellationToken)
        {
            var articleRepo = _dataStore.GetRepository<Article>();

            var entity = await articleRepo.FirstOrDefaultAsync(new ArticleFromIdSpecification(request.ArticleId), cancellationToken: cancellationToken);

            entity.Publish();

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
