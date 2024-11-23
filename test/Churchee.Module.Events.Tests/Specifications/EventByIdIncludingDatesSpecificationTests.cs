using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using FluentAssertions;
using Moq;

namespace Churchee.Module.Events.Tests.Specifications
{
    public class EventByIdIncludingDatesSpecificationTests
    {
        [Fact]
        public void EventByIdIncludingDatesSpecification_FiltersCorrectly()
        {
            var testEvent = new Event.Builder()
                .SetApplicationTenantId(Guid.NewGuid())
                .SetParentId(Guid.NewGuid())
                .SetParentSlug("/events")
                .SetPageTypeId(Guid.NewGuid())
                .SetTitle("Test Event")
                .SetDates(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1))
                .Build();

            var testEvent2 = new Event.Builder()
                .SetApplicationTenantId(Guid.NewGuid())
                .SetParentId(Guid.NewGuid())
                .SetParentSlug("/events")
                .SetPageTypeId(Guid.NewGuid())
                .SetTitle("Test Event 2")
                .SetDates(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1))
                .Build();

            var events = new List<Event> { testEvent, testEvent2 };

            var mockEventRepository = new Mock<IRepository<Event>>();

            var specification = new EventByIdIncludingDatesSpecification(testEvent.Id);

            var results = specification.Evaluate(events).ToList();

            results.Count.Should().Be(1);
        }
    }
}
