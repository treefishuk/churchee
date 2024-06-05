﻿using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.Pages.Queries.GetPageContent
{
    public class GetPageContentRequestHandler : IRequestHandler<GetPageContentRequest, IEnumerable<GetPageContentResponseItem>>
    {

        private readonly IDataStore _storage;

        public GetPageContentRequestHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<GetPageContentResponseItem>> Handle(GetPageContentRequest request, CancellationToken cancellationToken)
        {
            return await _storage.GetRepository<PageContent>()
                .ApplySpecification(new PageContentForPageSpecification(request.PageId))
                .OrderBy(o => o.PageTypeContent.Order)
                .Select(s => new GetPageContentResponseItem
                {
                    PageTypeContentId = s.PageTypeContentId,
                    Title = s.PageTypeContent.Name,
                    Value = s.Value,
                    Type = s.PageTypeContent.Type
                }).ToListAsync();
        }
    }
}
