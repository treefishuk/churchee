using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Commands.CreatePageTypeContent
{
    public class CreatePageTypeContentCommandHandler : IRequestHandler<CreatePageTypeContentCommand, CommandResponse>
    {

        private readonly IDataStore _storage;

        public CreatePageTypeContentCommandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(CreatePageTypeContentCommand request, CancellationToken cancellationToken)
        {
            var pageType = await _storage.GetRepository<PageType>().FirstOrDefaultAsync(new GetPageTypeByIdAndIncludePageTypeContentSpecification(request.PageTypeId), cancellationToken);

            pageType.AddPageTypeContent(Guid.NewGuid(), request.Name, request.Type, request.Required, request.Order);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
