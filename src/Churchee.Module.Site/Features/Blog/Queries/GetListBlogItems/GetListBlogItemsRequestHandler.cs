using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Responses;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Queries.GetListBlogItems
{
    public class GetListBlogItemsRequestHandler : IRequestHandler<GetListBlogItemsRequest, DataTableResponse<GetListBlogItemsResponseItem>>
    {
        private readonly IDataStore _storage;

        public GetListBlogItemsRequestHandler(IDataStore storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task<DataTableResponse<GetListBlogItemsResponseItem>> Handle(GetListBlogItemsRequest request, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<Article>();

            return await repo.GetDataTableResponseAsync(new AllArticlesSpecification(), request.OrderBy, request.OrderByDirection, request.Skip, request.Take, s => new GetListBlogItemsResponseItem
            {
                Id = s.Id,
                Title = s.Title,
                Modified = s.ModifiedDate,
                Url = s.Url,
                Published = s.Published
            }, cancellationToken);
        }
    }

}
