using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, CommandResponse>
    {

        private readonly IDataStore _storage;

        public DeleteArticleCommandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
        {
            await _storage.GetRepository<Article>().SoftDelete(request.ArticleId);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
