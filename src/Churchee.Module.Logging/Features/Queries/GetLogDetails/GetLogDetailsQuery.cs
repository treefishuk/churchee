using MediatR;

namespace Churchee.Module.Logging.Features.Queries
{
    internal class GetLogDetailsQuery : IRequest<GetLogDetailsResponse>
    {
        public GetLogDetailsQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }

    }
}
