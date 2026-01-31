using Ardalis.Specification;
using Churchee.Module.Tokens.Entities;

namespace Churchee.Module.Tokens.Specifications
{

    public class GetTokenByKeySpecification : Specification<Token>
    {
        public GetTokenByKeySpecification(string key, Guid applicationTenantId)
        {
            Query.IgnoreQueryFilters();

            Query.Where(x => x.Key == key && x.ApplicationTenantId == applicationTenantId).AsNoTracking();
        }

        public GetTokenByKeySpecification(string key)
        {
            Query.Where(x => x.Key == key).AsNoTracking();
        }

    }
}
