using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Tenancy.Features.Churches.Queries
{
    public class GetInfoForEditQuery : IRequest<GetInfoForEditResponse>
    {
        public GetInfoForEditQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
