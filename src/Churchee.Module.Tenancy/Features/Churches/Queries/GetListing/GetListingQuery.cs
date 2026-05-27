using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Tenancy.Features.Churches.Queries
{
    public class GetListingQuery : IRequest<IEnumerable<GetListingQueryResponseItem>>
    {
    }
}
