using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Churchee.Module.Facebook.Events.Features.Queries
{
    public class GetTokenUrlQuery : IRequest<string>
    {
        public GetTokenUrlQuery(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}
