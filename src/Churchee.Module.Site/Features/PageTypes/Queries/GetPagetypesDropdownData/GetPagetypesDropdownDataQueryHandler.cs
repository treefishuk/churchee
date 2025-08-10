using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Churchee.Module.Site.Features.PageTypes.Queries
{

    public class GetPageTypesDropdownDataQueryHandler : IRequestHandler<GetPagetypesDropdownDataQuery, IEnumerable<SelectListItem>>
    {

        private readonly IDataStore _storage;

        public GetPageTypesDropdownDataQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<SelectListItem>> Handle(GetPagetypesDropdownDataQuery request, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<PageType>();

            return await repo.GetListAsync(new NonProtectedPageTypesSpecification(), s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }, cancellationToken);
        }
    }
}
