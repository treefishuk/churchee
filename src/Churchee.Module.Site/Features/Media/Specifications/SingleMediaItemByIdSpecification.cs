using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Features.Media.Specifications
{

    public class SingleMediaItemByIdSpecification : Specification<MediaItem>
    {
        public SingleMediaItemByIdSpecification(Guid mediaItemid)
        {
            Query.Where(x => x.Id == mediaItemid);
        }

    }
}
