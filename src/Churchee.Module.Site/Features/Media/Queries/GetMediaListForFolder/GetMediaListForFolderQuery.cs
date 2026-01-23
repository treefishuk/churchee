using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Queries
{
    public class GetMediaListForFolderQuery : IRequest<IEnumerable<GetMediaListForFolderQueryResponseItem>>
    {
        public GetMediaListForFolderQuery(Guid? mediaFolderId)
        {
            MediaFolderId = mediaFolderId;
        }

        public Guid? MediaFolderId { get; set; }
    }
}
