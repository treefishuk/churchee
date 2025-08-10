using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Logging.Entities;
using Churchee.Module.Logging.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Module.Logging.Jobs
{
    internal class ClearLogsJob : ISystemJob
    {
        private readonly LogsDBContext _dbContext;

        public ClearLogsJob(LogsDBContext dataStore)
        {
            _dbContext = dataStore;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var cutOffDate = DateTime.UtcNow.AddDays(-30);

            await _dbContext.Set<Log>()
                        .Where(log => log.TimeStamp > cutOffDate)
                        .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
