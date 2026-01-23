using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Churchee.Module.Facebook.Events.Features.Queries
{
    public class GetAuthUrlQuery : IRequest<string>
    {
        public GetAuthUrlQuery(string domain, string pageId)
        {
            Domain = domain;
            PageId = pageId;
        }

        public string Domain { get; set; }

        public string PageId { get; set; }

    }
}
