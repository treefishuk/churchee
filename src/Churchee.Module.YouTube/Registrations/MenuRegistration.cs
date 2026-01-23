using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;

namespace Churchee.Module.YouTube.Registrations
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
                    .AddChild(new MenuItem("YouTube Videos", "/management/integrations/youtube", "smart_display"))
                };

                return list;


            }
        }
    }
}
