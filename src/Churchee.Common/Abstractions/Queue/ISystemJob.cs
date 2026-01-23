using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Queue
{
    public interface ISystemJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
