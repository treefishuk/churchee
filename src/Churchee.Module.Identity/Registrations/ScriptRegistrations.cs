using Churchee.Common.Abstractions.Extensibility;
using System.Collections.Generic;

namespace Churchee.Module.Identity.Registrations
{
    public class ScriptRegistrations : IScriptRegistrations
    {
        public List<string> Scripts => ["/_content/Churchee.Module.Identity/scripts/qrcode.js", "/_content/Churchee.Module.Identity/scripts/churchee-identity.js"];
    }
}
