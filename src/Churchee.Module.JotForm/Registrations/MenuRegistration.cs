using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;

namespace Churchee.Module.Jotform.Registrations
{
    public class MenuRegistration : IMenuRegistration
    {
        public IEnumerable<MenuItem> MenuItems
        {
            get
            {
                var list = new List<MenuItem>
                {
                    new MenuItem("Integrations", "/management/integrations", "integration_instructions", 100)
                    .AddChild(new MenuItem("Jotform", "/management/integrations/jotform", "dynamic_form"))
                };

                return list;


            }
        }
    }
}
