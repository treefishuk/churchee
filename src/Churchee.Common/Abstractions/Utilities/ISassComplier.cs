using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Utilities
{
    public interface ISassComplier
    {
        Task<string> CompileStringAsync(string scss, IEnumerable<string> loadPaths = null, bool compressed = true, CancellationToken ct = default);
    }
}
