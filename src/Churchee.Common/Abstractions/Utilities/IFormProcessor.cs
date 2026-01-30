using System.Collections.Generic;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Utilities
{
    public interface IFormProcessor
    {
        Task<bool> ProcessForm(IDictionary<string, string> formData);
    }
}
