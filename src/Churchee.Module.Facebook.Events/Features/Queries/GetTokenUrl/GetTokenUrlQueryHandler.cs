using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Churchee.Module.Facebook.Events.Features.Queries
{
    public class GetTokenUrlQueryHandler : IRequestHandler<GetTokenUrlQuery, string>
    {
        public Task<string> Handle(GetTokenUrlQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
