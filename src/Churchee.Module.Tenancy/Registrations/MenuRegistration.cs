using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;

namespace Churchee.Module.Tenancy.Registration
{
    public class MenuRegistration : IMenuRegistration
    {
        public List<MenuItem> MenuItems
        {
            get
            {
                var list = new List<MenuItem>
                {
                    new MenuItem("Configuration", "/management/cconfiguration", "settings", 1000)
                    .AddChild(new MenuItem("Churches", "/management/configuration/churches", "church", 1, "SysAdmin"))
                };

                return list;


            }
        }
    }
}
