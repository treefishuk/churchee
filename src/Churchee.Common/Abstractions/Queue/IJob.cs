using System;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Queue
{
    public interface IJob
    {
        Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken);
    }
}
