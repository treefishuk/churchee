using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;

namespace Churchee.Module.Dashboard.Tests.Specifications
{
    public class PageViewsAfterDateBatchSpecificationTests
    {
        [Fact]
        public void Cursor_excludes_rows_before_or_at_the_last_row()
        {
            var startDate = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastViewedAt = startDate.AddHours(1);
            var cursor = new PageView(Guid.NewGuid()) { ViewedAt = lastViewedAt };
            var spec = new PageViewsAfterDateBatchSpecification(startDate, 100, lastViewedAt, cursor.Id);
            var criteria = spec.WhereExpressions.Last().Filter.Compile();

            var before = new PageView(Guid.NewGuid()) { ViewedAt = lastViewedAt.AddMinutes(-1) };
            var after = new PageView(Guid.NewGuid()) { ViewedAt = lastViewedAt.AddMinutes(1) };

            Assert.False(criteria(before));
            Assert.False(criteria(cursor));
            Assert.True(criteria(after));
        }
    }
}