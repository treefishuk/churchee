using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;

        public UpdateArticleCommandHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            var articleRepo = _dataStore.GetRepository<Article>();

            var entity = await articleRepo.FirstOrDefaultAsync(new ArticleFromIdSpecification(request.Id), cancellationToken: cancellationToken);

            entity.UpdateInfo(request.Title, request.Description, request.ParentPageId);
            entity.SetContent(request.Content);
            entity.SetPublishDate(request.PublishOnDate);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
