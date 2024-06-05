using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Commands.CreatePageTypeContent
{
    public class CreatePageTypeContentComandHandler : IRequestHandler<CreatePageTypeContentComand, CommandResponse>
    {

        private readonly IDataStore _storage;

        public CreatePageTypeContentComandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(CreatePageTypeContentComand request, CancellationToken cancellationToken)
        {
            var pageType = _storage.GetRepository<PageType>()
                .ApplySpecification(new IncludePageTypeContentSpecification())
                .FirstOrDefault(f => f.Id == request.PageTypeId);

            pageType.AddPageTypeContent(Guid.NewGuid(), request.Name, request.Type, request.Required);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
