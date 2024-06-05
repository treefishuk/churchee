using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class DeleteMediaItemCommand : IRequest<CommandResponse>
    {
        public DeleteMediaItemCommand(Guid mediaItemId)
        {
            MediaItemId = mediaItemId;
        }

        public Guid MediaItemId { get; private set; }
    }
}
