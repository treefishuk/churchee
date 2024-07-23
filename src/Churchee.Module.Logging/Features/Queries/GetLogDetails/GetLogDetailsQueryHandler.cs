using Churchee.Module.Logging.Entities;
using Churchee.Module.Logging.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Module.Logging.Features.Queries
{
    internal class GetLogDetailsQueryHandler : IRequestHandler<GetLogDetailsQuery, GetLogDetailsResponse>
    {

        private readonly LogsDBContext _logsDBContext;

        public GetLogDetailsQueryHandler(LogsDBContext logsDBContext)
        {
            _logsDBContext = logsDBContext;
        }

        public async Task<GetLogDetailsResponse> Handle(GetLogDetailsQuery request, CancellationToken cancellationToken)
        {
            return await _logsDBContext.Set<Log>().Where(w => w.Id == request.Id).Select(s => new GetLogDetailsResponse
            {
                Id = s.Id,
                Exception = s.Exception,
                Level = s.Level,
                Message = s.Message,
                MessageTemplate = s.MessageTemplate,
                PropertiesString = s.Properties,
                TimeStamp = s.TimeStamp
            }).FirstOrDefaultAsync(cancellationToken);
        }
    }
}