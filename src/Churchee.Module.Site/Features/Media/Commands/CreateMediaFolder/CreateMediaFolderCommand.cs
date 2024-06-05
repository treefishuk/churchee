using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class CreateMediaFolderCommand : IRequest<CommandResponse>
    {
        public CreateMediaFolderCommand(Guid? parentId, string name)
        {
            ParentId = parentId;
            Name = name;
        }

        public Guid? ParentId { get; set; }

        public string Name { get; set; }


    }
}
