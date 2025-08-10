using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;

namespace Churchee.Module.Site.Registration
{
    public class MenuRegistration : IMenuRegistration
    {

        private readonly IDataStore _store;

        public MenuRegistration(IDataStore store)
        {
            _store = store;
        }

        public IEnumerable<MenuItem> MenuItems
        {
            get
            {

                if (!_store.GetRepository<ApplicationTenant>().Any())
                {
                    return Enumerable.Empty<MenuItem>();
                }

                var list = new List<MenuItem>
                {
                    new MenuItem("Website", "/management/pages", "devices")
                    .AddChild(new MenuItem("Pages", "/management/pages", "web"))
                    .AddChild(new MenuItem("Page Types", "/management/pagetypes", "view_quilt", 1, "Developer"))
                    .AddChild(new MenuItem("Articles", "/management/articles", "article"))
                    .AddChild(new MenuItem("Media", "/management/media", "image"))
                    .AddChild(new MenuItem("Templates", "/management/templates", "code", 1, "Developer"))
                    .AddChild(new MenuItem("Styles", "/management/styles", "data_object", 1, "Developer"))
                    .AddChild(new MenuItem("Podcasts", "/management/podcasts", "graphic_eq"))
                    .AddChild(new MenuItem("Videos", "/management/videos", "smart_display"))
                    .AddChild(new MenuItem("Events", "/management/events", "event"))
                };

                return list;


            }
        }
    }
}
