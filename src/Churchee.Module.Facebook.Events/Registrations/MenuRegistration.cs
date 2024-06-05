using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;

namespace Churchee.Module.Facebook.Events.Registrations
{
    public class MenuRegistration : IMenuRegistration
    {
        public List<MenuItem> MenuItems
        {
            get
            {
                var list = new List<MenuItem>
                {
                    new MenuItem("Integrations", "/management/integrations", "integration_instructions", 100)
                    .AddChild(new MenuItem("Facebook Events", "/management/integrations/facebook-events", "facebook"))
                };

                return list;


            }
        }
    }
}
