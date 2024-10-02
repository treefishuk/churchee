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
            var testEvent = new Event(Guid.NewGuid(), Guid.NewGuid(), "/events", Guid.NewGuid(), "", "", "Test Event", "", "", "", "", "", "", "", null, null, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1), "");
            var testEvent2 = new Event(Guid.NewGuid(), Guid.NewGuid(), "/events", Guid.NewGuid(), "", "", "Test Event 2", "", "", "", "", "", "", "", null, null, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1), "");

            var events = new List<Event> { testEvent, testEvent2 };

            var mockEventRepository = new Mock<IRepository<Event>>();

            var specification = new EventByIdIncludingDatesSpecification(testEvent.Id);

            var results = specification.Evaluate(events).ToList();

            results.Count.Should().Be(1);
        }
    }
}
