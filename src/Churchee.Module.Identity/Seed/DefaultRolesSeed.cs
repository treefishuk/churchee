using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using System;
using System.Collections.Generic;

namespace Churchee.Module.Identity.Seed
{
    public class DefaultRolesSeed : ISeedData
    {
        public int Order => 10;

        public void SeedData(IDataStore storage)
        {
            var repo = storage.GetRepository<ApplicationRole>();

            var roles = new List<ApplicationRole> {
                new(Guid.Parse("c0ff7d6f-d8db-44f9-9e6b-9da94acab150"), "SysAdmin"),
                new(Guid.Parse("2df6dd09-8021-4079-87f9-eee8ec01c640"), "Admin", true)
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
