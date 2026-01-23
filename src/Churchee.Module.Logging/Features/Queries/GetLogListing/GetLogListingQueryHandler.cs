using Churchee.Common.Abstractions;
using Churchee.Module.Logging.Entities;
using Churchee.Module.Logging.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Module.Logging.Features.Queries
{
    internal class GetLogListingQueryHandler : IRequestHandler<GetLogListingQuery, DataTableResponse<GetLogListingResponseItem>>
    {

        private readonly LogsDBContext _logsDBContext;

        public GetLogListingQueryHandler(LogsDBContext logsDBContext)
        {
            _logsDBContext = logsDBContext;
        }

        public async Task<DataTableResponse<GetLogListingResponseItem>> Handle(GetLogListingQuery request, CancellationToken cancellationToken)
        {
            var count = await _logsDBContext.Set<Log>().CountAsync(cancellationToken);

            string orderby = $"{request.OrderBy} {request.OrderByDirection}";

            var data = await _logsDBContext.Set<Log>()
                 .OrderBy(orderby)
                 .Skip(request.Skip)
                 .Take(request.Take)
                 .Select(s => new GetLogListingResponseItem
                 {
                     Id = s.Id,
                     Message = s.Message,
                     Level = s.Level,
                     TimeStamp = s.TimeStamp
                 })
                 .ToListAsync(cancellationToken);

            return new DataTableResponse<GetLogListingResponseItem>
            {
                RecordsTotal = count,
                RecordsFiltered = count,
                Draw = request.Take,
                Data = data
            };
        }
    }
}
