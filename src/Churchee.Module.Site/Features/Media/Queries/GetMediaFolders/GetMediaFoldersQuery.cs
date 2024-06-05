using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Queries
{
    public class GetMediaFoldersQuery : IRequest<IEnumerable<GetMediaFoldersQueryResponseItem>>
    {
        public GetMediaFoldersQuery(Guid? parentId)
        {
            ParentId = parentId;
        }

        public Guid? ParentId { get; set; }
    }
}
