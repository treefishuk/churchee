using Churchee.Common.Abstractions.Extensibility;

namespace Churchee.Module.Hangfire.Registrations
{
    public class ModuleRegistration : IModule
    {
        public string Name => "Hangfire";
    }
}
