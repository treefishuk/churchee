using Churchee.Common.Abstractions.Extensibility;

namespace Churchee.Module.UI.Registrations
{
    public class ScriptRegistrations : IScriptRegistrations
    {
        public List<string> Scripts => ["/_content/Churchee.Module.UI/scripts/churchee-ui-interop.js"];
    }
}
