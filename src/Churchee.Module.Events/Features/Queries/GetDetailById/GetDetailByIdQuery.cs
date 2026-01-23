using MediatR;

namespace Churchee.Module.Events.Features.Queries
{
    public class GetDetailByIdQuery : IRequest<GetDetailByIdResponse>
    {
        public GetDetailByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

    }
}
