using System.Threading.Tasks;

namespace Churchee.Module.Identity.Abstractions
{
    public interface IIdentitySeed
    {
        /// <summary>
        /// Create the seed data.
        /// </summary>
        Task CreateAsync();
    }
}
