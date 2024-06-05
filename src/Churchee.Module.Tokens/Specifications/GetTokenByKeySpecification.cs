using Ardalis.Specification;
using Churchee.Module.Tokens.Entities;
using System.Linq;

namespace Churchee.Module.Tokens.Specifications
{

    public class GetTokenByKeySpecification : Specification<Token>
    {
        public GetTokenByKeySpecification(string key, Guid applicationTenantId)
        {
            Query.IgnoreQueryFilters();

            Query.Where(x => x.Key == key && x.ApplicationTenantId == applicationTenantId).AsNoTracking();
        }

    }
}
