using Churchee.Module.UI.Models;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetParentArticlePagesDropdownDataQuery : IRequest<IEnumerable<DropdownInput>>
    {
    }
}
