using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Extensibility;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;

namespace Churchee.Module.Reviews.Registration
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
                    return [];
                }

                var list = new List<MenuItem>
                {
                    new MenuItem("Website", "/management/pages", "devices")
                    .AddChild(new MenuItem("Reviews", "/management/reviews", "star", 1000))
                };

                return list;


            }
        }
    }
}
