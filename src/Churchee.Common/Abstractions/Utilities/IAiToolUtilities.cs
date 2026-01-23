using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Utilities
{
    public interface IAiToolUtilities
    {
        Task<string> GenerateAltTextAsync(Stream imageStream, CancellationToken cancellationToken);
    }
}
