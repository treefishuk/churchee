using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Churchee.Module.Site.Features.PageTypes.Queries
{

    public class GetPagetypesDropdownDataQuery : IRequest<IEnumerable<SelectListItem>>
    {
    }
}
