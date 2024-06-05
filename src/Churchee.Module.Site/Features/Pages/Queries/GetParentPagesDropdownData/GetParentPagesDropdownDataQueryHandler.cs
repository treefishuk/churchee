using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.UI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetParentPagesDropdownDataQueryHandler : IRequestHandler<GetParentPagesDropdownDataQuery, IEnumerable<DropdownInput>>
    {

        private readonly IDataStore _storage;

        public GetParentPagesDropdownDataQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<DropdownInput>> Handle(GetParentPagesDropdownDataQuery request, CancellationToken cancellationToken)
        {
            return await _storage.GetRepository<Page>()
                .GetQueryable()
                .Select(s => new DropdownInput { Title = s.Title, Value = s.Id.ToString() })
                .ToListAsync(cancellationToken);
        }
    }
}
