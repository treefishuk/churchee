using Churchee.Module.Site.Features.Templates.Responses;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.Templates.Queries.GetTemplateById
{
    public class GetTemplateByIdQuery : IRequest<TemplateDetailResponse>
    {
        public GetTemplateByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
