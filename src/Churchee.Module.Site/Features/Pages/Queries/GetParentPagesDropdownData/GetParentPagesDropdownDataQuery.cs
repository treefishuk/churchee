using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Module.UI.Models;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetParentPagesDropdownDataQuery : IRequest<IEnumerable<DropdownInput>>
    {
    }
}
