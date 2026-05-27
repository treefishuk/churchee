using Churchee.Module.UI.Models;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetParentPagesDropdownDataQuery : IRequest<IEnumerable<DropdownInput>>
    {
        public GetParentPagesDropdownDataQuery()
        {
            CurrentPage = null;
        }

        public GetParentPagesDropdownDataQuery(Guid? currentPage)
        {
            CurrentPage = currentPage;
        }

        public Guid? CurrentPage { get; set; }
    }
}
