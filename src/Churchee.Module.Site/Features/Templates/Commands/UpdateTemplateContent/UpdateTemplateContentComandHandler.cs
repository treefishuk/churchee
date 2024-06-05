using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Templates.Commands.UpdateTemplateContent
{
    public class UpdateTemplateContentComandHandler : IRequestHandler<UpdateTemplateContentComand, CommandResponse>
    {

        private readonly IDataStore _storage;

        public UpdateTemplateContentComandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(UpdateTemplateContentComand request, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<ViewTemplate>();

            var template = repo.GetById(request.TemplateId);

            template.SetContent(request.Content);

            repo.Update(template);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
