using System;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;

namespace Churchee.Module.Identity.Seed
{
    public class DefaultRolesSeed : ISeedData
    {
        public int Order => 10;

        public void SeedData(IDataStore storage)
        {
            var repo = storage.GetRepository<ApplicationRole>();

            bool empty = !repo.AnyWithFiltersDisabled(a => a != null);

            if (empty)
            {
                repo.Create(new ApplicationRole(Guid.Parse("c0ff7d6f-d8db-44f9-9e6b-9da94acab150"), "SysAdmin"));

                storage.SaveChanges();
            };

        }
    }
}
