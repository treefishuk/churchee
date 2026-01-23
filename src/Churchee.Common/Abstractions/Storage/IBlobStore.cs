using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Common.Storage
{
    public interface IBlobStore
    {
        /// <summary>
        /// Save Image Stream
        /// </summary>
        /// <returns>The fullPath as it may have been changed</returns>
        Task<string> SaveAsync(Guid applicationTenantId, string fullPath, Stream stream, bool overrideExisting = false, CancellationToken cancellationToken = default);

        Task<Stream> GetReadStreamAsync(Guid applicationTenantId, string fullPath, CancellationToken cancellationToken = default);

        Task WriteChunksAsync(Guid applicationTenantId, string fullPath, IBrowserFile file, IProgress<double> progress, CancellationToken cancellationToken = default);
    }
}
