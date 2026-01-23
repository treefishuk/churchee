using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Templates.Responses;
using MediatR;

namespace Churchee.Module.Site.Features.Templates.Queries.GetTemplateById
{
    public class GetTemplateByIdQueryHandler : IRequestHandler<GetTemplateByIdQuery, TemplateDetailResponse>
    {
        private readonly IDataStore _storage;

        public GetTemplateByIdQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<TemplateDetailResponse> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var dbItem = await _storage.GetRepository<ViewTemplate>()
                                        .GetByIdAsync(request.Id, cancellationToken);

            return new TemplateDetailResponse() { Id = dbItem.Id, Content = dbItem.Content };
        }
    }
}
