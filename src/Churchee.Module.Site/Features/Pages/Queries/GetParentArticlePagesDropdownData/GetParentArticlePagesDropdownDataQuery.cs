using Churchee.Module.UI.Models;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetParentArticlePagesDropdownDataQuery : IRequest<IEnumerable<DropdownInput>>
    {
    }
}
