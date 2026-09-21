using Churchee.CQRS.Abstractions;
using Churchee.Module.UI.Models;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetAllPagesDropdownDataQuery : IRequest<IEnumerable<DropdownInput>>
    {
        public GetAllPagesDropdownDataQuery()
        {
            SearchFilter = string.Empty;
        }

        public GetAllPagesDropdownDataQuery(string searchFilter)
        {
            SearchFilter = searchFilter;
        }

        public string SearchFilter { get; set; }
    }
}
