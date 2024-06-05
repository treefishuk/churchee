using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;

namespace Churchee.Module.Podcasts.Anchor.Registrations
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
                    .AddChild(new MenuItem("Anchor", "/management/integrations/anchor", "anchor"))
                };

                return list;


            }
        }
    }
}
