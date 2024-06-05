using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.PageTypes.Queries
{

    public class GetPagetypesDropdownDataQueryHandler : IRequestHandler<GetPagetypesDropdownDataQuery, IEnumerable<SelectListItem>>
    {

        private readonly IDataStore _storage;

        public GetPagetypesDropdownDataQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<SelectListItem>> Handle(GetPagetypesDropdownDataQuery request, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<PageType>();

            return await repo.GetQueryable().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToListAsync(cancellationToken);

        }
    }
}
