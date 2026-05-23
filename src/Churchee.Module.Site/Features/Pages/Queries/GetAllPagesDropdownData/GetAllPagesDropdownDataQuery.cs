using Churchee.Module.UI.Models;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetAllPagesDropdownDataQuery : IRequest<IEnumerable<DropdownInput>>
    {

    }
}
