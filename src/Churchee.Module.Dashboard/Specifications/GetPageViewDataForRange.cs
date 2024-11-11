using Ardalis.Specification;
using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Specifications
{
    internal class GetPageViewDataForRange : Specification<PageView>
    {
        public GetPageViewDataForRange(DateTime start)
        {
            Query.Where(x => x.ViewedAt > start);
        }
    }
}
