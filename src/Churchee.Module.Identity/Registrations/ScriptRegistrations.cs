using Churchee.Common.Abstractions.Extensibility;

namespace Churchee.Module.Identity.Registrations
{
    public class ScriptRegistrations : IScriptRegistrations
    {
        public ICollection<string> Scripts => ["/_content/Churchee.Module.Identity/scripts/qrcode.js", "/_content/Churchee.Module.Identity/scripts/churchee-identity.js"];
    }
}
