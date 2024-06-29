using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Identity.Entities;

namespace Churchee.Module.Site.Seed
{
    public class DefaultRolesSeed : ISeedData
    {
        public int Order => 10;

        public void SeedData(IDataStore storage)
        {
            var repo = storage.GetRepository<ApplicationRole>();

            var roles = new List<ApplicationRole> {
                new(Guid.Parse("39e6cbe0-886c-43e3-971f-4c0999e58bb7"), "Developer", true)
            };

            foreach (var role in roles)
            {
                bool empty = !repo.AnyWithFiltersDisabled(a => a.Id == role.Id);

                if (empty)
                {
                    repo.Create(role);

                    storage.SaveChanges();
                }
            }
        }
    }
}
