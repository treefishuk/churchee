using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;

namespace Churchee.Module.Podcasts.Spotify.Registrations
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
                    .AddChild(new MenuItem("Spotify Podcasts", "/management/integrations/spotify", "graphic_eq"))
                };

                return list;


            }
        }
    }
}
