using Churchee.Common.Abstractions;
using MediatR;

namespace Churchee.Module.Podcasts.Features.Queries
{
    public class GetListingQuery : IRequest<DataTableResponse<GetListingQueryResponseItem>>
    {
        public GetListingQuery(int skip, int take, string searchText, string orderBy, string orderByDirection, int draw)
        {
            Skip = skip;
            Take = take;
            SearchText = searchText;
            OrderBy = orderBy;
            OrderByDirection = orderByDirection;
            Draw = draw;
        }

        public int Draw { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public string SearchText { get; set; }

        public string OrderBy { get; set; }

        public string OrderByDirection { get; set; }

    }
}
