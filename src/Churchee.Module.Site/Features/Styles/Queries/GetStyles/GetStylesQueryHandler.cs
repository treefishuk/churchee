﻿using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.Styles.Queries
{
    public class GetStylesQueryHandler : IRequestHandler<GetStylesQuery, string>
    {

        private readonly IDataStore _storage;

        public GetStylesQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }


        public async Task<string> Handle(GetStylesQuery request, CancellationToken cancellationToken)
        {
            var css = await _storage.GetRepository<Css>().GetQueryable().FirstOrDefaultAsync();

            return css.Styles;
        }
    }
}
