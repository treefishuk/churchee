using Churchee.Common.Extensibility;

namespace Churchee.Common.Abstractions.Extensibility
{
    public interface IMenuRegistration
    {
        IEnumerable<MenuItem> MenuItems { get; }
    }
}
