using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand, CommandResponse>
    {

        private readonly IDataStore _storage;

        public UpdatePageCommandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
        {

            var page = _storage.GetRepository<Page>()
                .ApplySpecification(new PageWithContentAndPropertiesSpecification(request.PageId))
                .FirstOrDefault();

            page.UpdateInfo(request.Title, request.Description, request.ParentId);
            page.UpdateContent(request.Content);

            await _storage.SaveChangesAsync(cancellationToken);

            if (request.Publish)
            {
                page.Publish();
            }

            if (request.Unpublish)
            {
                page.Unpublish();
            }

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
