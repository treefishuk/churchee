using System.Collections.Generic;
using Churchee.Common.Extensibility;

namespace Churchee.Common.Abstractions.Extensibility
{
    public interface IMenuRegistration
    {
        List<MenuItem> MenuItems { get; }
    }
}
