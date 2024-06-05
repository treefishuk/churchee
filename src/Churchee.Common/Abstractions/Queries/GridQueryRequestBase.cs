using MediatR;

namespace Churchee.Common.Abstractions.Queries
{
    public abstract class GridQueryRequestBase<TResponseType> : IRequest<DataTableResponse<TResponseType>>
    {
        public int Draw { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public string SearchText { get; set; }

        public string OrderBy { get; set; }

        public string OrderByDirection { get; set; }

    }
}
