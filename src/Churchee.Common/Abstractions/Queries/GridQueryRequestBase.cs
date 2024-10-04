﻿using MediatR;
using System;

namespace Churchee.Common.Abstractions.Queries
{
    public abstract class GridQueryRequestBase<TResponseType> : IRequest<DataTableResponse<TResponseType>>
    {
        protected GridQueryRequestBase(int skip, int take, string searchText, string orderBy)
        {
            Skip = skip;
            Take = take;
            SearchText = searchText;

            if (string.IsNullOrEmpty(orderBy))
            {
                OrderBy = string.Empty;
                OrderByDirection = "asc";

                return;
            }

            if (orderBy.Contains(' '))
            {
                var split = orderBy.Split(" ");
                OrderBy = split[0];
                OrderByDirection = split[1].ToLowerInvariant();

                if (OrderByDirection != "asc" && OrderByDirection != "desc")
                {
                    throw new ArgumentOutOfRangeException(nameof(orderBy), message: "Unsupported order direction");
                }

                return;
            }

            OrderBy = orderBy;
            OrderByDirection = "asc";
        }

        public int Skip { get; set; }

        public int Take { get; set; }

        public string SearchText { get; set; }

        public string OrderBy { get; set; }

        public string OrderByDirection { get; set; }
    }
}
