using Churchee.Common.Extensibility;
using System.Collections.Generic;

namespace Churchee.Common.Abstractions.Extensibility
{
    public interface IMenuRegistration
    {
        IEnumerable<MenuItem> MenuItems { get; }
    }
}
