using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;
using System.Collections.Generic;

namespace Churchee.Module.Identity.Registrations
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
                    .AddChild(new MenuItem("Contributors", "/management/configuration/contributors", "supervisor_account", 1, "Admin"))
                };

                return list;


            }
        }
    }
}
