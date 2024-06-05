using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Common.Storage
{
    public interface IBlobStore
    {
        Task SaveAsync(Guid applicationTenantId, string fullPath, Stream stream, bool overrideExisting = false, CancellationToken cancellationToken = default);

        Task<Stream> GetAsync(Guid applicationTenantId, string fullPath, CancellationToken cancellationToken = default);
    }
}
