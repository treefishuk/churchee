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

        public List<MenuItem> MenuItems
        {
            get
            {

                if (!_store.GetRepository<ApplicationTenant>().Any())
                {
                    return Enumerable.Empty<MenuItem>().ToList();
                }

                var list = new List<MenuItem>
                {
                    new MenuItem("Website", "/management/pages", "devices")
                    .AddChild(new MenuItem("Pages", "/management/pages", "web"))
                    .AddChild(new MenuItem("Page Types", "/management/pagetypes", "view_quilt"))
                    .AddChild(new MenuItem("Feed", "/management/feed", "article"))
                    .AddChild(new MenuItem("Media", "/management/media", "image"))
                    .AddChild(new MenuItem("Templates", "/management/templates", "code"))
                    .AddChild(new MenuItem("Styles", "/management/styles", "data_object"))
                    .AddChild(new MenuItem("Podcasts", "/management/podcasts", "graphic_eq"))
                    .AddChild(new MenuItem("Events", "/management/events", "event"))

                };

                return list;


            }
        }
    }
}
