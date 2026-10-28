using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Events.Tests.Specifications
{
    public class EventByIdAndTenantSpecificationTests
    {
        [Fact]
        public void EventByIdAndTenantSpecification_FiltersCorrectly()
        {
            var tenant1Id = Guid.NewGuid();
            var tenant2Id = Guid.NewGuid();

            var testEvent = new Event.Builder()
                .SetApplicationTenantId(tenant1Id)
                .SetParentId(Guid.NewGuid())
                .SetParentSlug("/events")
                .SetPageTypeId(Guid.NewGuid())
                .SetTitle("Test Event")
                .SetDates(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1))
                .Build();

            var testEvent2 = new Event.Builder()
                .SetApplicationTenantId(tenant2Id)
                .SetParentId(Guid.NewGuid())
                .SetParentSlug("/events")
                .SetPageTypeId(Guid.NewGuid())
                .SetTitle("Test Event 2")
                .SetDates(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1))
                .Build();

            var events = new List<Event> { testEvent, testEvent2 };

            var specification = new EventByIdAndTenantSpecification(testEvent.Id, tenant1Id);

            var results = specification.Evaluate(events).ToList();

            results.Count.Should().Be(1);
        }
    }
}
