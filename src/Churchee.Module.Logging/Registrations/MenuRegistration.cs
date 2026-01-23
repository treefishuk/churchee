using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;
using System.Collections.Generic;

namespace Churchee.Module.Logging.Registration
{
    public class MenuRegistration : IMenuRegistration
    {
        public IEnumerable<MenuItem> MenuItems
        {
            get
            {
                var list = new List<MenuItem>
                {
                    new MenuItem("Configuration", "/management/configuration", "settings", 1000)
                    .AddChild(new MenuItem("Logs", "/management/configuration/logs", "list", 100, "SysAdmin"))
                };

                return list;
            }
        }
    }
}

