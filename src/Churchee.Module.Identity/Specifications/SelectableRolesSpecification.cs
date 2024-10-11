using Ardalis.Specification;

namespace Churchee.Module.Events.Specifications
{
    public class SelectableRolesSpecification : Specification<ApplicationRole>
    {
        public SelectableRolesSpecification()
        {
            Query.Where(w => w.Selectable);
        }
    }
}
